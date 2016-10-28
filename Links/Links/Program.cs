using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Links
{
    class Url
    {
        static public string GetStatusCodeForConnectToUrl(System.String str)
        {
            Uri siteUri = new Uri(str);
            HttpWebRequest webRequest = (HttpWebRequest)(WebRequest.Create(siteUri.GetLeftPart(UriPartial.Query)));

            try
            {
                webRequest.Timeout = 10000;
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)(webRequest.GetResponse());
                webRequest.Abort();
                //Console.WriteLine(myHttpWebResponse.StatusCode);
                myHttpWebResponse.Close();
                return myHttpWebResponse.StatusCode.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(webRequest.RequestUri);
                Console.WriteLine((System.String)(ex.Message));
                return (System.String)(ex.Message);
            }
        }
    }

    class App
    {


        /*static public Array Resize(ref Array arr, int length)
        {
            Array.Resize(ref newArray, length);
        }*/
        static public string[] ExtractURLs(string str)
        {
            // match.Groups["name"].Value - URL Name
            // match.Groups["url"].Value - URI
            string RegexPattern = @"<a.*?href=[""'](?<url>.*?)[""'].*?>(?<name>.*?)</a>";

                 // Find matches.
            System.Text.RegularExpressions.MatchCollection matches
             = System.Text.RegularExpressions.Regex.Matches(str, RegexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            string[] MatchList = new string[matches.Count];

            // Report on each match.
            int c = 0;
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                MatchList[c] = match.Groups["url"].Value;
                c++;
            }

            return MatchList;
        }

        static public string GetHTMLDocument(string url)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                string content = sr.ReadToEnd();
                sr.Close();
                return content;
            }
            catch(Exception ex)
            {
                return "";
            }
        }

        static public bool IsRepetition(ref string[] links, string link, int ownNumber = -1)
        {
            for (int i = links.Length - 1; i != -1; --i)
            {
                if (link == links[i] && i != ownNumber)
                {
                    return true;
                }
            }
            return false;
        }

        static public bool CheckUrl(System.String str)
        {
            Uri siteUri = new Uri(str);
            HttpWebRequest webRequest = (HttpWebRequest)(WebRequest.Create(siteUri.GetLeftPart(UriPartial.Query)));

            try
            {
                webRequest.Timeout = 10000;
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)(webRequest.GetResponse());
                webRequest.Abort();
                Console.WriteLine(myHttpWebResponse.StatusCode);
                myHttpWebResponse.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(webRequest.RequestUri);
                Console.WriteLine((System.String)(ex.Message));
                return false;
            }
        }

        /*static public string GetUrlDomain(string url)
        {
            return string;
        }*/


        static public void PoolsLinks(ref string[] links)
        {
            StreamWriter log = new StreamWriter("log.txt");
            for(int i = 0; i != links.Length; ++i)
            {
                log.WriteLine(links[i] + " - " + Url.GetStatusCodeForConnectToUrl(links[i]));
            }
            log.Close();
        }


        static public string[] GetListAtTwoList(string[] firstList, string[] secondList)
        {
            string[] newList = new string[firstList.Length + secondList.Length];
            int count = 0;
            int number = 0;
            for (; count != firstList.Length; ++count)
            {
                newList[count] = firstList[count];
            }
            for(int i = 0; i != secondList.Length; ++i)
            {
                if (!IsRepetition(ref firstList, secondList[i]))
                {
                    newList[count] = secondList[i];
                    ++count;
                }
                else { ++number; }
                
            }
            Array.Resize(ref newList, newList.Length - number);
            return newList;
        }

        static public string GetAbsoluteLink(string link, string domain)
        {
            string url = "http://" + domain;
            if (link[0] != '/')
            {
                url += '/';
            }
            return link.Contains("#") ? url + "/" + link : url + link;
        }

        static public string ParseUrl(string link, string domain)
        {
            char[] delimiterChars = { '\\', '/', ':' };
            if (link.Contains("@") )
            {
                return "";
            }
            if (link.Contains(".ru") || link.Contains(".com"))
            {
                var list1 = link.Split(delimiterChars);
                if (list1[0] == "http" || list1[0] == "https")
                {
                    Uri uri = new Uri(link);
                    bool isContains = uri.Authority.Contains(domain);
                    if (!isContains)
                    {
                        return "";
                    }
                }
                if (link.Contains(domain))
                {
                    return link;
                }
                else { return ""; }
            }
            var list = link.Split(delimiterChars);
            bool isAbsoluteLink = false;
            
            if(list[0] == "http")
            {
                isAbsoluteLink = true;
            }
            if (isAbsoluteLink)
            {
                Uri uri = new Uri(link);
                bool isContains = uri.Authority.Contains(domain);
                if (!isContains)
                {
                    return "";
                }
                return uri.AbsolutePath;
            }
            return GetAbsoluteLink(link, domain);

        }

        static public void RemoveAt(ref string[] arr, int index, int length)
        {
            Array.Clear(arr, index, length);
            for(int i = index; i < arr.Length - 1; ++i)
            {
                arr.SetValue(arr.GetValue(i + 1), i);
            }
            Array.Resize(ref arr, arr.Length - 1);
        }

        static public string[] GetSortedArrayLinks(string[] links, string domain)
        {
            for (int i = links.Length - 1; i != -1; --i)
            {
                string link = (string)links.GetValue(i);
                string url = ParseUrl(link, domain);
                if (url == "" || IsRepetition(ref links, url, i))
                {
                    //Array.Clear(links, i, 1);
                    RemoveAt(ref links, i, 1);
                }
                else
                {
                    links.SetValue(url, i);
                }

            }



            return links;
        }

        static void Main(string[] args)
        {
            string content = GetHTMLDocument("http://bizmagazine.ru/");

            string[] list = ExtractURLs(content);

            System.Uri mainDomain = new Uri(content);
            list = GetSortedArrayLinks(list, mainDomain.Authority);
            
            StreamWriter log= new StreamWriter("log.txt");
            for (int count = 0; count != list.Length; ++count)
            {
                string statusCode = Url.GetStatusCodeForConnectToUrl(list[count]);
                log.WriteLine(list[count] + "  -  " + statusCode);
                Console.WriteLine(list[count] + "  -  " + statusCode);
                if (statusCode == "OK")
                {
                    //Console.WriteLine(list[count]);
                    string newContent = GetHTMLDocument(list[count]);
                    string[] newList = ExtractURLs(newContent);
                    newList = GetSortedArrayLinks(newList, mainDomain.Authority);
                    list = GetListAtTwoList(list, newList);
                }
            }


           /* for(int i = 0; i != list.Length; ++i)
            {
                Console.WriteLine(list.GetValue(i));
            }*/
            Console.WriteLine(mainDomain.Authority);

            // Url.CheckUrl("https://lenta.ru/rubrics/meia/");

           // log.Close();
            StreamWriter file = new StreamWriter("text.txt");
            string[] list2 = { "kakawka", "brokkoli", "http://dom.lenta.ru/", "http://lenta.ru/articles/2016/10/27/krym_nash/" };
            var list1 = GetListAtTwoList(list, list2);

            for (int i = 0; i != list1.Length; ++i)
            {
                Console.WriteLine(list1.GetValue(i));
            }

            for (int i = 0; i != list1.Length; ++i)
            {
                file.WriteLine((string)list1.GetValue(i));
            }
            //Console.WriteLine();
            file.Close();

            //PoolsLinks(ref list);


            System.Uri exempleDomain = new Uri("http://hosts/");

            Console.WriteLine(exempleDomain.Authority);
            Console.WriteLine(ParseUrl("http://facebook.com/lenta.ru", "lenta.ru"));
            Console.ReadKey();
        }
    }
}