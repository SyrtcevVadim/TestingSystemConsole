using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestingSystemConsole
{
    /// <summary>
    /// Класс для чтения информации из файла с эталонными ответами
    /// </summary>
    class AnswersReader
    {
        /// <summary>
        /// Поток, связанный с файлом эталонных ответов
        /// </summary>
        private FileStream answersFile;

        /// <summary>
        /// Поток чтения, связанный с файлом
        /// </summary>
        private StreamReader reader;

        public AnswersReader(string pathToAnswersFile)
        {
            answersFile = new FileStream(pathToAnswersFile, FileMode.Open);
            reader = new StreamReader(answersFile);
        }

        /// <summary>
        /// Считываем очередную эталонные ответы для соответствующего тестового случая
        /// </summary>
        public string GetNextAnswerData()
        {
            
            // Прокручиваемся к первому ответу
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
    }
}
