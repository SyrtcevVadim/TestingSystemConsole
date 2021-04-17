using System;
using System.IO;

namespace TestingSystemConsole
{
    /// <summary>
    /// Класс для считывания данных из файла с тестовыми данными
    /// </summary>
    class TestsReader
    {
        /// <summary>
        /// Поток, связанный с файлом тестов для пользовательской программы
        /// </summary>
        private FileStream testsFile;

        /// <summary>
        /// Поток для чтения, связанный с файлом тестов
        /// </summary>
        private StreamReader reader;

        private int testsQuantity;
        /// <summary>
        /// Количество тестов в файле
        /// </summary>
        public int TestsQuantity
        {
            get
            {
                return testsQuantity;
            }
        }
        
        private string currentTestName;
        /// <summary>
        /// Название текущего выполняемого теста
        /// </summary>
        public string CurrentTestName
        {
            get
            {
                return currentTestName;
            }
        }

        /// <summary>
        /// Флаг, показывающий, остались ли в файле тесты для считывания или нет.
        /// </summary>
        private bool hasRemainingTests;

        public TestsReader(string pathToTestsFile)
        {
            // Создаем поток, связанный с файлом тестов
            testsFile = new FileStream(pathToTestsFile, FileMode.Open);
            
            // Создаем для этого файла поток для чтения
            reader = new StreamReader(testsFile);

            // Считываем количество тестов в файле
            testsQuantity = Convert.ToInt32(reader.ReadLine());
            Console.WriteLine("В файле записано {0} тестов!", testsQuantity);
            if(testsQuantity > 0)
            {
                hasRemainingTests = true;   
            }
            else
            {
                hasRemainingTests = false;
            }
        }

        ~TestsReader()
        {
            reader.Close();
            testsFile.Close();
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
                Console.WriteLine("Мы добрались до очередного теста!");
                string testPrototype = reader.ReadLine();
                Console.WriteLine("Прототип текущего теста: {0}", testPrototype);
                currentTestName = testPrototype.Substring(testPrototype.IndexOf(' ') + 1);
                Console.WriteLine("Название текущего теста: {0}", currentTestName);

                // Готовимся записывать тестовые данные
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
    }
}
