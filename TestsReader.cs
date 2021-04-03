using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace TestingSystemConsole
{
    /// <summary>
    /// Класс для считывания тестовых данных из файла
    /// </summary>
    class TestsReader
    {
        private FileStream testsFile;
        /// <summary>
        /// Поток для чтения, связанный с файлом с тестами
        /// </summary>
        private StreamReader reader;

        /// <summary>
        /// Количество тестов в файле
        /// </summary>
        private uint testsQuantity;

        /// <summary>
        /// Название текущего выполняемого теста
        /// </summary>
        private string currentTestName;

        /// <summary>
        /// Флаг, показывающий, остались ли в файле тесты для считывания или нет.
        /// </summary>
        private bool hasRemainingTests;
        public TestsReader(string pathToTestsFile)
        {
            testsFile = new FileStream(pathToTestsFile, FileMode.Open);
            
            reader = new StreamReader(testsFile);
            // Считываем количество тестов в файле
            testsQuantity = Convert.ToUInt32(reader.ReadLine());
            Console.WriteLine("В файле записано {0} тестов!", testsQuantity);
            hasRemainingTests = true;
        }

        /// <summary>
        /// Получает информацию из следующего теста в файле
        /// </summary>
        public string GetNextTestData()
        {
            if (hasRemainingTests)
            {
                Console.WriteLine("В файле с тестами остались данные!");
                // Двигаем указатель к началу следующего теста
                while (reader.Peek() != '#')
                {
                    reader.ReadLine();
                }
                Console.WriteLine("Мы добрались до теста!");
                string testPrototype = reader.ReadLine();
                Console.WriteLine("Прототип текущего теста: {0}", testPrototype);
                currentTestName = testPrototype.Substring(testPrototype.IndexOf(' ') + 1);
                Console.WriteLine("Название текущего теста: {0}", currentTestName);
                // Готовимся записывать тестирующую информацию
                string currentTestData = "";
                while (!reader.EndOfStream && reader.Peek() != '#')
                {
                    currentTestData += reader.ReadLine() + "\n";
                }
                if (reader.EndOfStream)
                {
                    Console.WriteLine("Находимся в конце потока!");
                    hasRemainingTests = false;
                }
                return currentTestData;
            }
            else
            {
                Console.WriteLine("В файле не осталось тестовых данных!");
                return "";
            }
        }

        /// <summary>
        /// Возвращает количество тестов в файле
        /// </summary>
        public uint GetTestsQuantity()
        {
            return testsQuantity;
        }

    }
}
