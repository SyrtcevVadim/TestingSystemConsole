using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestingSystemConsole
{
    /// <summary>
    /// Класс для чтения информации из файла ограничений 
    /// </summary>
    class RestrictionsReader
    {

        private int memoryLimitInMegabytes;
        /// <summary>
        /// Ограничение по используемой в пользовательской программе памяти в мегабайтах
        /// </summary>
        public int MemoryLimitInMegabytes
        {
            get
            {
                return memoryLimitInMegabytes;
            }
            set
            {
                memoryLimitInMegabytes = value;
            }
        }

        private int timeLimitInMilliseconds;
        /// <summary>
        /// Ограничение по времени работы пользовательской программы для каждого теста в миллисекундах
        /// </summary>
        public int TimeLimitInMilliseconds
        {
            get
            {
                return timeLimitInMilliseconds;
            }
            set
            {
                timeLimitInMilliseconds = value;
            }
        }

        public RestrictionsReader(string pathToRestrictionsFile)
        {
            FileStream restrictionsFile = new FileStream(pathToRestrictionsFile, FileMode.Open);
            StreamReader reader = new StreamReader(restrictionsFile);

            // Получаем данные из файла ограничений
            string timeLimitString = reader.ReadLine();
            string memoryLimitString = reader.ReadLine();
            TimeLimitInMilliseconds = Convert.ToInt32(timeLimitString.Substring(timeLimitString.IndexOf(' ')));
            MemoryLimitInMegabytes = Convert.ToInt32(memoryLimitString.Substring(memoryLimitString.IndexOf(' ')));

            reader.Close();
        }
    }
}
