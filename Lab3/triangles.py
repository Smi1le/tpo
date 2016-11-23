# coding=utf-8
'''
        1. Напишите модульные тесты для функций calculateSquare(), calculateAngle(), isTriangle(), getType().
        2. В некоторых из этих функций есть ошибки, поэтому правильно написанные тесты должны падать (обнаруживать эти ошибки)
        3. Исправьте ошибки. Теперь тесты должны проходить (у всех тестов статус ОК).
        4. В указанных функциях есть места, написанные не очень эффективно. Перепишите их. 
           Удостоверьтесь, что ничего не сломано — все ваши тесты проходят.
        5. Посчитайте покрытие тестами. Для этого используйте инструмент coverage https://pypi.python.org/pypi/coverage
'''

import unittest
import math

class Triangle:
        '''
                a, b, c — стороны треугольника
                класс позволяет определить, является ли это треугольником
                какого типа этот треугольник
                посчитать периметр и площадь
        '''
        def __init__(self, a, b, c):
                self.triangle = [a, b, c]
        
        def getA(self):
                return self.triangle[0]
                
        def getB(self):
                return self.triangle[1]
                
        def getC(self):
                return self.triangle[2]
                
        def calculatePerimeter(self):
                '''
                        расчет периметра
                '''
                return sum(self.triangle)
                
        def calculateSquare(self):
                '''
                        расчет площади по формуле Герона
                '''
                if(self.isTriangle() == False):
                        return False
                
                a = self.triangle[0]
                b = self.triangle[1]
                c = self.triangle[2]
                
                p = (a+b+c)
                p = p/2.0
                S = math.sqrt(p*(p-a)*(p-b)*(p-c))
                return S
        
        def calculateAngle(self, angle):
                '''
                        Расчет угла по теореме косинусов.
                        В качестве параметров передается alpha, beta, gamma — название угла, который нужно посчитать. 
                        Угол находится напротив соответствующей стороны (a, b, c)
                        Возвращает величину угла в градусах
                '''
                if(self.isTriangle() == False):
                        return False 
                
                a = self.triangle[0]
                b = self.triangle[1]
                c = self.triangle[2]
                
                firstSide = 1
                secondSide = 1
                searchSide = 1
                
                if (angle == 'alpha'):
                        secondSide = c
                        firstSide = b
                        searchSide = a
                elif (angle == 'beta'):
                        firstSide = a
                        secondSide = c
                        searchSide = b
                elif (angle == 'gamma'):
                        firstSide = a
                        secondSide = b
                        searchSide = c
                else:
                        return False
        
                f = (firstSide**2 + secondSide**2 - searchSide**2)/(2.0*firstSide*secondSide)
                print("f = ", f)
                return math.degrees(math.acos(f))
        
        def isTriangle(self):
                '''
                        Проверка, что треугольник с введенными сторонами вообще может существовать
                '''
                fail = False
                a = self.triangle[0]
                b = self.triangle[1]
                c = self.triangle[2]
                
                if (a <= 0):
                        fail = True
                elif (b <= 0):
                        fail = True
                elif (c <= 0):
                        fail = True
                
                if (a >= b+c):
                        fail = True
                elif (b >= a+c):
                        fail = True
                elif (c >= a+b):
                        fail = True
                
                if (fail):
                        return False
                else:
                        return True



        '''
                Возвращает тип треугольника:
                common — просто треугольник
                isosceles — равнобедренный
                equilateral — равносторонний
        '''     
        def getTypeForSide(self):

                if(self.isTriangle() == False):
                        return "not triangle" 
                
                type = 'common'
                a = self.triangle[0]
                b = self.triangle[1]
                c = self.triangle[2]
                
                if ((a == b) and (a == c)):
                        type = 'equilateral'
                elif (a == b) and (a != c):
                        type = 'isosceles'
                elif (a == c) and (a != b):
                        type = 'isosceles'
                elif (b == c) and (b != a):
                        type = 'isosceles'
                return type


        '''
                Возвращает тип треугольника:
                right — прямоугольный
                acute-angled — остроугольный
                obtuse — тупоугольный
        '''
        def getTypeForDegrees(self):
                degrees = []
                
                if(self.isTriangle()):
                        degrees = [self.calculateAngle("alpha"), self.calculateAngle("beta"), self.calculateAngle("gamma")]
                else:
                        return "not triangle"

                type = 'right'

                a = self.triangle[0]
                b = self.triangle[1]
                c = self.triangle[2]
               
                if (a**2 == b**2 + c**2):
                        type = 'right'
                elif (b*2 == a**2 + c**2):
                        type = 'right'
                elif (c**2 == a**2 + b**2):
                        type = 'right'
                elif (degrees[0] < 90 and degrees[1] < 90 and degrees[2] < 90):
                        type = 'acute-angled'
                elif ((degrees[0] > 90) or (degrees[1] > 90) or (degrees[2] > 90)):
                        type = 'obtuse'
                
                return type


class TriangleTest(unittest.TestCase):
        def setUp(self):
                print "Test started"
                
        def tearDown(self):
                print "Test finished"

        # Проверяем, что на корректных значениях программа работает
        def testIsTriangle(self):
                t = Triangle(2, 3, 4)
                self.assertTrue(t.isTriangle())

        # значение некорректное, это не треугольник, функция isTriangle() должна вернуть false
        def testIsNotTriangle(self):
                t = Triangle(2, 3, 5)
                self.assertFalse(t.isTriangle())

        #Проверяем типы треугольника со сторонами 5, 11, 5 
        def testCheckAllCornersIsoscelesTriangle(self):
                t = Triangle(5, 11, 5)
                self.assertTrue(t.getTypeForSide() == "not triangle")
                self.assertTrue(t.getTypeForDegrees() == "not triangle")
        
        #Проверяем углы треугольника, который теоретически не может существовать
        def testCheckAllCornersTriangleThatMayNotExist(self):
                t = Triangle(5, 11, 5)
                self.assertFalse(t.calculateAngle("alpha"))
                self.assertFalse(t.calculateAngle("beta"))
                self.assertFalse(t.calculateAngle("gamma"))
        

        # Проверяем, что геттеры программы возвращают правильные значения
        def testCheckGetFunctions(self):
                t = Triangle(2, 3, 4)
                self.assertTrue(t.getA() == 2)
                self.assertTrue(t.getB() == 3)
                self.assertTrue(t.getC() == 4)

        # одно из значений меньше нуля, это не треугольник, функция isTriangle() должна вернуть false
        def testIsNotTriangleButLengthOneSideLessThenZero(self):
                t = Triangle(-1, 3, 5)
                self.assertFalse(t.isTriangle())

        # одно из значений равно нулю, это не треугольник, функция isTriangle() должна вернуть false
        def testIsNotTriangleButLengthOneSideIsZero(self):
                t = Triangle(0, 3, 5)
                self.assertFalse(t.isTriangle())

        # Проверяем что на корректных значениях программа правильно высчитывает площадь треугольника
        def testCalculateTheAreaTriangleOnTheRightTriangle(self):
                t = Triangle(5, 3, 5)
                fault = abs(t.calculateSquare() - 7.1545);
                self.assertTrue(fault < 0.001)

        # Проверяем что на некорректных значениях программа не высчитывает площадь треугольника
        def testCalculateTheAreaTriangleOnTheWrongTriangle(self):
                t = Triangle(5, 11, 5)
                self.assertFalse(t.calculateSquare())

        #TODO: Подумать над тестом где значения треугольника не правильные, и нужно подсчитать площадь

         # Проверяем что на корректных значениях программа правильно высчитывает периметр треугольника
        def testCalculateThePerimeterTriangle(self):
                t = Triangle(5, 3, 5)
                self.assertTrue(t.calculatePerimeter() == 13)

                
        #|-------------------Проверяем стороны треугольника----------------------------|


                
        # Проверяем что по введенным значениям треугольник является равносторонним
        def testCheckTypeTriangleInEquilateral(self):
                t = Triangle(5, 5, 5)
                self.assertTrue(t.getTypeForSide() == "equilateral")

        # Проверяем что по введенным значениям треугольник является равнобедренным
        def testCheckTypeTriangleInIsosceles(self):
                t = Triangle(5, 3, 5)
                self.assertTrue(t.getTypeForSide() == "isosceles")

         # Проверяем что по введенным значениям треугольник является обычным
        def testCheckTypeTriangleInCommon(self):
                t = Triangle(4, 3, 5)
                self.assertTrue(t.getTypeForSide() == "common")

        # Проверяем что по введенным значениям треугольник является прямоугольным
        def testCheckTypeTriangleInRight(self):
                t = Triangle(3, 4, 5)
                self.assertTrue(t.getTypeForDegrees() == "right")
        
        # Проверяем что по введенным значениям треугольник является тупоугольным
        def testCheckTypeTriangleInAcute_Angled(self):
                t = Triangle(5, 5, 5)
                self.assertTrue(t.getTypeForDegrees() == "acute-angled")

        # Проверяем что по введенным значениям треугольник является остроугольным
        def testCheckTypeTriangleInObtuse(self):
                t = Triangle(2, 3, 4)
                self.assertTrue(t.getTypeForDegrees() == "obtuse")
        
        #|-----------------------------------------------------------------------------|
        
        # Проверяем что у равностороннего треуголника все углы равны по 60 градусов
        def testFromAllCornersEquilateralTriangleEqualto60Degrees(self):
                t = Triangle(6, 6, 6)
                self.assertTrue(abs(t.calculateAngle("alpha") - 60) < 0.00001)
                self.assertTrue(abs(t.calculateAngle("beta") - 60) < 0.00001)
                self.assertTrue(abs(t.calculateAngle("gamma") - 60) < 0.00001)

        #Проверяем углы у равнобедренного треугольника со сторонами 5, 3, 5
        def testCheckAllCornersIsoscelesTriangleForSide5and3and5(self):
                t = Triangle(5, 3, 5)

                self.assertTrue(abs(t.calculateAngle("alpha") - 72.54) < 0.01)
                self.assertTrue(abs(t.calculateAngle("beta") - 34.92) < 0.01)
                self.assertTrue(abs(t.calculateAngle("gamma") - 72.54) < 0.01)


        
                
                
if __name__ == '__main__':
        unittest.main()
        
