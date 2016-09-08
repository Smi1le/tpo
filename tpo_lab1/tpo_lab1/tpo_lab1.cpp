#include "stdafx.h"
#include <string>
#include <iostream>

using namespace std;

double CharToDouble(char *S)
{
	int k = 1;
	double firstNumber = 0;
	double secondNumber = 1;
	for (int i = 0; i < strlen(S); ++i)
	{
		if (k < 0) secondNumber = secondNumber * 10;
		if (S[i] == '.' || S[i] == ',')
			k = -k;
		else
			firstNumber = firstNumber * 10 + (S[i] - '0');
	}
	return firstNumber / secondNumber;
}

std::string GetTypeTriangle(double lengthA, double lengthB, double lengthC)
{

	if (lengthA == 0 || lengthB == 0 || lengthC == 0 ||
		(lengthA + lengthB) <= lengthC || (lengthA + lengthC) <= lengthB || (lengthB + lengthC) <= lengthA)
	{
		throw invalid_argument("Такого треугольника не существует");
	}
	if (lengthA == lengthB && lengthA == lengthC && lengthB == lengthC)
	{
		return string("Равносторонний");
	}
	else if (lengthA == lengthB || lengthA == lengthC || lengthB == lengthC)
	{
		return string("Равнобедренный");
	}
	else
	{
		return string("Обычный");
	}
}


int main(int argc, char* argv[])
{
	setlocale(LC_ALL, "rus");
	if (argc != 4)
	{
		cout << "Укажите длины сторон в качестве параметров. Формат ввода: triangle.exe a b c" << endl;
		return EXIT_FAILURE;
	}
	try
	{
		for (int j = 1; j != 4; ++j)
		{
			for (int i = 0; i != strlen(argv[1]); ++i)
			{
				if (isalpha(argv[j][i]))
				{
					throw invalid_argument("Пожалуйста, указывайте в качестве аргументов числа");
					//cout << "Буква" << endl;
				}
			}
		}
		cout << GetTypeTriangle(CharToDouble(argv[1]), CharToDouble(argv[2]), CharToDouble(argv[3])) << endl;
		return EXIT_SUCCESS;
	}
	catch (invalid_argument const &e)
	{
		cout << "Error! " << e.what() << endl;
		return EXIT_FAILURE;
	}
}

