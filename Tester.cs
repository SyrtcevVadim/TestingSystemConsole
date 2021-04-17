using System;
using System.IO;
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

        /// <summary>
        /// Читает информацию из файла тестов
        /// </summary>
        private TestsReader testsReader;

        /// <summary>
        /// Читает информацию из файлов ответов
        /// </summary>
        private AnswersReader answersReader;

        /// <summary>
        /// Читает информацию из файлов ограничений
        /// </summary>
        private RestrictionsReader restrictionsReader;
   
        /// <param name="pathToUserExecutableFile">Путь к файлу пользовательской программы</param>
        /// <param name="pathToTestsFile">Путь к файлу с тестовыми данными для программы</param>
        /// <param name="pathToAnswersFile">Путь к файлу с эталонными ответами на тестовые данные</param>
        /// <param name="pathToRestrictionsFile">Путь к файлу с ограничениями на выполнение программы</param>
        public Tester(string pathToUserExecutableFile, string pathToTestsFile, string pathToAnswersFile, string pathToRestrictionsFile ="")
        {
            // Создаем новый процесс для тестирования пользовательской программы
            userExecutable = new Process();
            // Настраиваем поток для работы с пользовательской программой
            ConfigureProcess(pathToUserExecutableFile);

            

            // Связываем файлы и объектами для их чтения
            testsReader = new TestsReader(pathToTestsFile);
            answersReader = new AnswersReader(pathToAnswersFile);
            if(pathToRestrictionsFile != "")
            {
                restrictionsReader = new RestrictionsReader(pathToRestrictionsFile);
            }
        }

        ~Tester()
        {
            // Освобождаем ресурсы, отведённые для потока
            userExecutable.Dispose();
        }

        /// <summary>
        /// Настраивает поток для тестирования пользовательской программы
        /// </summary>
        /// <param name="pathToUserExecutableFile"></param>
        private void ConfigureProcess(string pathToUserExecutableFile)
        {
            try
            {
                // Настраиваем поток на для тестирования пользовательской программы
                // Для запуска потока не нужно использовать оболочку операционной системы
                userExecutable.StartInfo.UseShellExecute = false;
                // Устанавливаем для потока путь к исполняемому файлу
                userExecutable.StartInfo.FileName = pathToUserExecutableFile;
                // Для работающей в потоке пользовательской программы не нужно создавать отдельное окно консоли
                userExecutable.StartInfo.CreateNoWindow = true;

                // Делаем возможным перенаправление стандартных потоков ввода/вывода
                userExecutable.StartInfo.RedirectStandardInput = true;
                userExecutable.StartInfo.RedirectStandardOutput = true;

                /// Устанавливаем высокий приоритет исполнения для данного потока(чтобы сократить накладные расходы на время тестирования программы)
                userExecutable.PriorityClass = ProcessPriorityClass.High;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        /// <summary>
        /// Начинает тестирование пользовательской программы
        /// </summary>
        public void Start()
        {
            // Получаем количество тестов для пользовательской программы
            int testsQuantity = testsReader.TestsQuantity;
            for(int counter = 0; counter < testsQuantity; counter++)
            {
                // Получаем тестовые данные
                string currentTest = testsReader.GetNextTestData();
                
                // Если процесс был запущен
                if(userExecutable.Start())
                {
                    StreamWriter processInput = userExecutable.StandardInput;
                    StreamReader processOutput = userExecutable.StandardOutput;

                    // Вводим в тестируемую программу данные
                    processInput.Write(currentTest);

                    Console.WriteLine("Был запущен тест: {0}", (counter + 1));

                    // Замеряем время работы программы
                    DateTime startTesting = DateTime.Now;
                    while(!userExecutable.HasExited)
                    {
                        // Ожидаем завершения работы тестируемой программы
                        if(DateTime.Now.Subtract(startTesting).TotalMilliseconds > restrictionsReader.TimeLimitInMilliseconds)
                        {
                            // Если время работы программы превысило ограничение по времени, останавливаем процесс
                            userExecutable.Kill();
                            Console.WriteLine("Программа не прошла {0} тест по времени.\nТекущее ограничение по времени: {1} мс!\n", (counter + 1), 
                                                                                                        restrictionsReader.TimeLimitInMilliseconds);
                            
                        }
                    }
                    
                    // Программа отработала в корректных временных рамках

                    // Получаем от программы данные выходного потока
                    string output = processOutput.ReadToEnd();
                    Console.WriteLine("Тест{0}\nВремя работы программы:{1} мс\n" +
                        "Результат: {2} \n\n", counter,
                                               userExecutable.TotalProcessorTime.TotalMilliseconds,
                                               output);
                }
                else
                {
                    Console.WriteLine("Процесс для теста {0} не был создан!!1", (counter + 1));
                }
            }
        }

    }
}
