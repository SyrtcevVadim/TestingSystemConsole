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
                Console.WriteLine("Error! Program requires 3 arguments!");
                return 1;
            }
            else if(args.Length > 4)
            {
                Console.WriteLine("Error! Program can handle only 4 arguments!");
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

                // Тестируем класс TestReader 
                TestsReader testsReader = new TestsReader(pathToTestsFile);
                for (uint i = 1; i <= testsReader.TestsQuantity; i++)
                {
                    Console.WriteLine(testsReader.GetNextTestData());
                }

                // Тестируем класс AnswersReader
                AnswersReader answersReader = new AnswersReader(pathToAnswersFile);
                Console.WriteLine("Эталонные ответы: ");
                Console.WriteLine(answersReader.GetNextAnswerData());
                Console.WriteLine(answersReader.GetNextAnswerData());

                // Тестируем класс RestrictionsReader
                RestrictionsReader restrictionsReader = new RestrictionsReader(pathToRestrictionsFile);
                Console.WriteLine("Ограничения по использованию памяти: {0} Мб", restrictionsReader.MemoryLimitInMegabytes);
                Console.WriteLine("Ограничения на время работы пользовательской программы: {0} мс", restrictionsReader.TimeLimitInMilliseconds);


            }

            return 0;
        }
    }
}
