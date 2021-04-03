using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TestingSystemConsole
{
    /// <summary>
    /// Класс, предназначенный для тестирования пользовательской программы на тестовых данных
    /// </summary>
    class Tester
    {
        /// <summary>
        /// Процесс, в котором работает пользовательская программа
        /// </summary>
        private Process userExecutable;

        private TestsReader testsReader;

        private AnswersReader answersReader;

        private RestrictionsReader restrictionsReader;

        private void ConfigureProcess(string pathToUserExecutableFile)
        {
            userExecutable.StartInfo.UseShellExecute = false;
            userExecutable.StartInfo.FileName = pathToUserExecutableFile;
            userExecutable.StartInfo.CreateNoWindow = true;
        }

        /// <summary>
        /// Конструктор объекта тестирующей системы
        /// </summary>
        /// <param name="pathToUserExecutableFile">Абсолютный или относительный путь к файлу пользовательской программы</param>
        /// <param name="pathToTestsFile">Абсолютный или относительный путь к файлу с тестовыми данными для программы</param>
        /// <param name="pathToAnswersFile">Абсолютный или относительный путь к файлу с эталонными ответами на тестовые данные</param>
        /// <param name="pathToRestrictionsFile">Абсолютный или относительный путь к файлу с ограничениями на выполнение программы</param>
        public Tester(string pathToUserExecutableFile, string pathToTestsFile, string pathToAnswersFile, string pathToRestrictionsFile ="")
        {
            userExecutable = new Process();
            // Настраиваем поток для работы с пользовательской программой
            ConfigureProcess(pathToUserExecutableFile);
            if(pathToRestrictionsFile != "")
            {
                // TODO RestrictionsReader
            }
        }

        /// <summary>
        /// Начинает тестирование пользовательской программы на тестовых данных
        /// </summary>
        public void Start()
        {

        }

    }
}
