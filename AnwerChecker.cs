using System;
using System.IO;
using System.Text.RegularExpressions;

namespace TestingSystemConsole
{
    /// <summary>
    /// Класс для чтения информации из файла с эталонными ответами
    /// </summary>
    class AnwerChecker
    {
        /// <summary>
        /// Поток, связанный с файлом эталонных ответов
        /// </summary>
        private FileStream answersFile;

        /// <summary>
        /// Поток чтения, связанный с файлом
        /// </summary>
        private StreamReader reader;

        public AnwerChecker(string pathToAnswersFile)
        {
            answersFile = new FileStream(pathToAnswersFile, FileMode.Open);
            reader = new StreamReader(answersFile);
        }

        ~AnwerChecker()
        {
            reader.Close();
            answersFile.Close();
        }

        /// <summary>
        /// Считываем очередную эталонные ответы для соответствующего тестового случая
        /// </summary>
        public string GetNextAnswerData()
        {
            // Двигаем поток к очередному ответу
            while(reader.Peek() != '#')
            {
                reader.ReadLine();
            }
            // Считываем прототип ответа
            Console.WriteLine(reader.ReadLine());
            string currentAnswerData = "";
            while(!reader.EndOfStream && reader.Peek() != '#')
            {
                currentAnswerData += reader.ReadLine() + "\n";
            }

            return currentAnswerData;
        }
    
        /// <summary>
        /// Проверяет, совпадает ли результат выполнения программы на тестовых данных с эталонными ответами
        /// </summary>
        /// <param name="result">Результат работы программы на входных тестовых данных</param>
        /// <returns>true, если программа выдала корректный ответ. false в противном случае</returns>
        public bool IsAnswerCorrect(string result)
        {

            // Удаляем из строк лишние пробельные символы(пробелы, символы новой линии, табуляции)
            string programOutput = Regex.Replace(result, @"\s+", " ");
            string correctOutput = Regex.Replace(GetNextAnswerData(), @"\s+", " ");

            // Удаляем пустые символы в начале и в конце строк
            programOutput = programOutput.Trim();
            correctOutput = correctOutput.Trim();

            // Посимвольно сравниваем результаты
            // Если количество символов в строках разное, то ответы никак не могут совпадать
            if(programOutput.Length != correctOutput.Length)
            {
                return false;
            }

            for(int i = 0; i < programOutput.Length; i++)
            {
                if(programOutput[i] != correctOutput[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
