#include "stdafx.h"
#include <string>
#include <iostream>

using namespace std;

std::string GetTypeTriangle(double lengthA, double lengthB, double lengthC)
{
	if (lengthA == 0 || lengthB == 0 || lengthC == 0 ||
		(lengthA + lengthB) <= lengthC || (lengthA + lengthC) <= lengthB || (lengthB + lengthC) <= lengthA)
	{
		throw invalid_argument("������ ������������ �� ����������");
	}
	if (lengthA == lengthB && lengthA == lengthC && lengthB == lengthC)
	{
		return string("��������������");
	}
	else if (lengthA == lengthB || lengthA == lengthC || lengthB == lengthC)
	{
		return string("��������������");
	}
	else
	{
		return string("�������");
	}
}


int main(int argc, char* argv[])
{
	setlocale(LC_ALL, "rus");
	if (argc != 4)
	{
		cout << "������� ����� ������ � �������� ����������. ������ �����: triangle.exe a b c" << endl;
		return EXIT_FAILURE;
	}
	try
	{
		cout << GetTypeTriangle(atof(argv[1]), atof(argv[2]), atof(argv[3])) << endl;
		return EXIT_SUCCESS;
	}
	catch (invalid_argument const &e)
	{
		cout << "Error! " << e.what() << endl;
		return EXIT_FAILURE;
	}
}

