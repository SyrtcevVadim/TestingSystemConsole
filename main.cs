using System;
using System.IO;
using System.Threading;

namespace TestingSystemConsole
{
    class Program
    {
        static int Main(string[] args)
        {
            string pathToUserExecutableFile;
            string pathToTestsFile;
            string pathToAnswersFile;
            string pathToRestrictionsFile;
            // Программа для запуска принимает 3 обязательных аргумента и 1 необязательный
            // 1 аргумент: путь к файлу с пользовательской программой
            // 2 аргумент: путь к файлу тестов для пользовательской программы
            // 3 аргумент: путь к файлу эталонных ответов на входные тестовые данные
            // 4 аргумент: путь к файлу с ограничениями
            if (args.Length < 3)
            {
                Console.WriteLine("Ошибка! Программа требует три обязательных аргумента!");
                return 1;
            }
            else if(args.Length > 4)
            {
                Console.WriteLine("Ошибка! Программа не может обработать больше 4 аргументов!");
            }
            else if(args.Length == 3)
            {
                pathToUserExecutableFile = args[0];
                pathToTestsFile = args[1];
                pathToAnswersFile = args[2];

                

                

            }
            else if(args.Length == 4)
            {
                pathToUserExecutableFile = args[0];
                pathToTestsFile = args[1];
                pathToAnswersFile = args[2];
                pathToRestrictionsFile = args[3];

                // Тестируем класс Tester
                Tester tester = new Tester(pathToUserExecutableFile,
                                           pathToTestsFile,
                                           pathToAnswersFile,
                                           pathToRestrictionsFile, @"C:/Projects/TestingSystemConsole/TestData/log.txt");

                // Начинаем тестирование
                tester.Start();
            }

            return 0;
        }
    }
}
