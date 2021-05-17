using System;
using System.IO;

namespace TestingSystemConsole
{
    /// <summary>
    /// Класс для считывания данных из файла с тестовыми данными
    /// </summary>
    class TestReader
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
        public int TestQuantity
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
        /// Номер текущего теста
        /// </summary>
        private int currentTestNumber;

        public TestReader(string pathToTestsFile)
        {
            // Открываем файл тестов
            testsFile = new FileStream(pathToTestsFile, FileMode.Open);
            
            // Создаем для этого файла поток для чтения
            reader = new StreamReader(testsFile);

            // Считываем количество тестов в файле
            testsQuantity = Convert.ToInt32(reader.ReadLine());
            //Console.WriteLine("В файле записано {0} тестов!", testsQuantity);
            if(testsQuantity > 0)
            {
                currentTestNumber = 1; 
            }
            else
            {
                throw new Exception("Файл тестов пуст!");
            }
        }

        ~TestReader()
        {
            reader.Close();
            testsFile.Close();
        }

        /// <summary>
        /// Получает информацию из следующего теста в файле
        /// </summary>
        public string GetNextTestData()
        {
            if (currentTestNumber <= testsQuantity)
            {
                //Console.WriteLine("В файле с тестами остались данные!");
                //Console.WriteLine("Считываем тест: {0}", currentTestNumber);
                // Двигаем указатель к началу следующего теста
                while (reader.Peek() != '#')
                {
                    reader.ReadLine();
                }
                string testPrototype = reader.ReadLine();
                currentTestName = testPrototype.Substring(testPrototype.IndexOf(' ') + 1);
                //Console.WriteLine("Название текущего теста: {0}", currentTestName);

                // Готовимся записывать тестовые данные
                string currentTestData = "";
                while (!reader.EndOfStream && reader.Peek() != '#')
                {
                    currentTestData += reader.ReadLine() + "\n";
                }
                //if (reader.EndOfStream)
                //{
                //    Console.WriteLine("Находимся в конце потока!");
                //}
                currentTestNumber++;
                return currentTestData;
            }
            else
            {
                //Console.WriteLine("В файле не осталось тестовых данных!");
                return "";
            }
        }
    }
}
