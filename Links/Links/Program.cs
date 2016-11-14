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
            try
            {
                Uri siteUri = new Uri(str);
                HttpWebRequest webRequest = (HttpWebRequest)(WebRequest.Create(siteUri.GetLeftPart(UriPartial.Query)));
                webRequest.Timeout = 10000;
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)(webRequest.GetResponse());
                webRequest.Abort();
                myHttpWebResponse.Close();
                return myHttpWebResponse.StatusCode.ToString();
            }
            catch (Exception ex)
            {
                return (System.String)(ex.Message);
            }
        }

        static public string[] ExtractURLs(string str)
        {
            string RegexPattern = @"<a.*?href=[""'](?<url>.*?)[""'].*?>(?<name>.*?)</a>";

            System.Text.RegularExpressions.MatchCollection matches
             = System.Text.RegularExpressions.Regex.Matches(str, RegexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            string[] MatchList = new string[matches.Count];

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
            catch (Exception ex)
            {

                return (string)ex.Message;
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

        static public string[] GetListAtTwoList(string[] firstList, string[] secondList)
        {
            string[] newList = new string[firstList.Length + secondList.Length];
            int count = 0;
            int number = 0;
            for (; count != firstList.Length; ++count)
            {
                newList[count] = firstList[count];
            }
            for (int i = 0; i != secondList.Length; ++i)
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
            string url;
            if (link.Length > 1)
            {
                url = link.Substring(0, 2) == "//" ? url = "http:" + domain : "http://" + domain;
            }
            else
            {
                url = "http://" + domain + link;
            }
            if (link.Length > 0 && link[0] != '/')
            {
                url += '/';
            }

            return link.Contains("#") ? url + "/" + link : url + link;
        }

        static public string GetCorrectLink(string link, string domain)
        {
            char[] delimiterChars = { '\\', '/', ':' };
            if (link.Contains("@"))
            {
                return "";
            }
            var list1 = link.Split(delimiterChars);
            if (list1.Length > 0 && (list1[0] == "http" || list1[0] == "https"))
            {
                return link;
            }
            else
            {
                string copy = GetAbsoluteLink(link, domain);
                
                try
                {
                    Uri uri = new Uri(copy);
                    return copy;
                }
                catch
                {
                    string str = link.Substring(0, 2) == "//" ? "http:" + domain + link : "http://" + domain + link;
                    return str;
                }
            
            }
        }

        static public void RemoveAt(ref string[] arr, int index, int length)
        {
            Array.Clear(arr, index, length);
            for (int i = index; i < arr.Length - 1; ++i)
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
                string url = GetCorrectLink(link, domain);
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


        static public string GetCorrectStartUrl(string link)
        {
            char[] delimiterChars = { '\\', '/', ':' };
            var list = link.Split(delimiterChars);
            if (list[0] == "http" || list[0] == "https")
            {
                return link;
            }
            else
            {
                return "http://" + link;
            }
        }

        static public void CheckWebSiteOnBrokenLinks(string startUrl)
        {
            StreamWriter badLog = new StreamWriter("badLog.txt");
            StreamWriter log = new StreamWriter("log.txt");
            badLog.Close();
            log.Close();
            string content = GetHTMLDocument(GetCorrectStartUrl(startUrl));

            string[] list = ExtractURLs(content);

            System.Uri mainDomain = new Uri(GetCorrectStartUrl(startUrl));
            list = GetSortedArrayLinks(list, mainDomain.Authority);
            
            for (int i = 0; i != list.Length; ++i)
            {
                Console.WriteLine(list.GetValue(i));
            }
           
            for (int count = 0; count != list.Length; ++count)
            {
                badLog = new StreamWriter("badLog.txt", true);
                log = new StreamWriter("log.txt", true);
                string statusCode = Url.GetStatusCodeForConnectToUrl(GetCorrectLink(list[count], mainDomain.Authority));
                log.WriteLine(list[count] + "  -  " + statusCode);
                Console.WriteLine(list[count] + "  -  " + statusCode);
                Uri newUri = null;
                bool flag = true;
                try
                {
                    newUri = new Uri(list[count]);
                }
                catch
                {
                    flag = false;
                }
                if (flag && statusCode == "OK" && (newUri.Authority == mainDomain.Authority))
                {
                    string newContent = GetHTMLDocument(list[count]);
                    string[] newList = ExtractURLs(newContent);
                    newList = GetSortedArrayLinks(newList, mainDomain.Authority);
                    list = GetListAtTwoList(list, newList);
                }
                else if (statusCode != "OK")
                {
                    //Console.WriteLine("----------------------------------------------------------------------");
                    badLog.WriteLine(list[count] + "  -  " + statusCode);
                }
                badLog.Close();
                log.Close();
            }
        }
    }

    class App
    {
        static void Main(string[] args)
        {
            /*if (args.Length != 1)
            {
                Console.WriteLine("Ошибка! Укажите адрес страницы в качестве параметра. Формат ввода link_checker.exe <http://path-to-site.com>");
                return;
            }*/
            string url = "http://links.testingcourse.ru";//args[0];
            string statusCode = Url.GetStatusCodeForConnectToUrl(Url.GetCorrectStartUrl(url));
            if (statusCode == "OK")
            {
                Url.CheckWebSiteOnBrokenLinks(Url.GetCorrectStartUrl(url));
            }
            else
            {
                Console.WriteLine(statusCode);
            }
        }
    }
}