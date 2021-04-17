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
        /// Поток, связанный с файлом результатов тестирования
        /// </summary>
        private FileStream logFile;

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
        private AnwerChecker answersReader;

        /// <summary>
        /// Читает информацию из файлов ограничений
        /// </summary>
        private RestrictionsReader restrictionsReader;

        /// <param name="pathToUserExecutableFile">Путь к файлу пользовательской программы</param>
        /// <param name="pathToTestsFile">Путь к файлу с тестовыми данными для программы</param>
        /// <param name="pathToAnswersFile">Путь к файлу с эталонными ответами на тестовые данные</param>
        /// <param name="pathToRestrictionsFile">Путь к файлу с ограничениями на выполнение программы</param>
        /// <param name="pathToLogFile">Путь к файлу с результатами тестирования</param>
        public Tester(string pathToUserExecutableFile, string pathToTestsFile, string pathToAnswersFile, string pathToRestrictionsFile, string pathToLogFile)
        {
            // Создаем новый процесс для тестирования пользовательской программы
            userExecutable = new Process();
            // Настраиваем поток для работы с пользовательской программой
            ConfigureProcess(pathToUserExecutableFile);

            // Создаем файл для сохранения результатов тестирования
            try
            {
                logFile = new FileStream(pathToLogFile, FileMode.Truncate);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            

            // Связываем файлы и объектами для их чтения
            testsReader = new TestsReader(pathToTestsFile);
            answersReader = new AnwerChecker(pathToAnswersFile);
            restrictionsReader = new RestrictionsReader(pathToRestrictionsFile);
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
            for (int counter = 0; counter < testsQuantity; counter++)
            {
                // Получаем тестовые данные
                string currentTest = testsReader.GetNextTestData();

                // Если процесс был запущен
                if (userExecutable.Start())
                {
                    // Получаем стандартный поток ввода/вывода тестируемой программы
                    StreamWriter processInput = userExecutable.StandardInput;
                    StreamReader processOutput = userExecutable.StandardOutput;


                    // Вводим в тестируемую программу данные
                    processInput.Write(currentTest);

                    Console.WriteLine("Был запущен тест: {0}", (counter + 1));

                    long memoryUsage = 0L;
                    // Замеряем время работы программы
                    DateTime startTestingTime = DateTime.Now;
                    while (!userExecutable.HasExited)
                    {
                        TimeSpan executionTime = (DateTime.Now.Subtract(startTestingTime));
                        // Ожидаем завершения работы тестируемой программы
                        if (executionTime.TotalMilliseconds > restrictionsReader.TimeLimitInMilliseconds)
                        {
                            // Если время работы программы превысило ограничение по времени, останавливаем процесс
                            userExecutable.Kill();
                            Console.WriteLine("Программа не прошла {0} тест по времени.\nТекущее ограничение по времени: {1} мс!\n", (counter + 1),
                                                                                                        restrictionsReader.TimeLimitInMilliseconds);
                            break;
                        }
                        // Замеряем количество используемой памяти в мегабайтах
                        memoryUsage = userExecutable.WorkingSet64 / (1024 * 1024);
                    }

                    // В случае, если программа не превысила ограничения по времени работы

                    // Получаем от программы данные выходного потока
                    string output = processOutput.ReadToEnd();
                    // Проверяем выходные данные на корректность
                    bool passed = answersReader.IsAnswerCorrect(output);

                    // Отображаем результаты тестирования в консоли
                    ShowLogInConsole(testsReader.CurrentTestName, userExecutable.TotalProcessorTime.TotalMilliseconds, memoryUsage, passed);

                    // Пишем результат тестирования в файл
                    SaveLogToFile(testsReader.CurrentTestName, userExecutable.TotalProcessorTime.TotalMilliseconds, memoryUsage, passed);
                }
                else
                {
                    Console.WriteLine("Процесс для теста {0} не был создан!!1", (counter + 1));
                }
            }
        }

        /// <summary>
        /// Сохраняет результат прохождения теста в выходной файл
        /// </summary>
        /// <param name="testName">Название теста</param>
        /// <param name="executionTime">Время, затраченное программой на выполнение теста</param>
        /// <param name="memoryUsage">Количество используемой программой памяти в мегабайтах</param>
        /// <param name="passed">Показывает, пройден программой данный тест или нет</param>
        public void SaveLogToFile(string testName, double executionTime, long memoryUsage, bool passed)
        {
            StreamWriter logStream = new StreamWriter(logFile);
            logStream.WriteLine(String.Format("Тест: {0} | Статус: {1} | Время исполнения: {2} мс | Объем использованной памяти {3} Мб |\n",
                                            testName, (passed) ? "пройден" : "не пройден", executionTime, memoryUsage));
            logStream.Flush();
        }

        /// <summary>
        /// Отображает результат прохождения теста в консоли
        /// </summary>
        /// <param name="testName">Название теста</param>
        /// <param name="executionTime">Время, затраченное программой на выполнение теста</param>
        /// <param name="memoryUsage">Количество используемой программой памяти в мегабайтах</param>
        /// <param name="passed">Показывае, пройден программой данный тест или нет</param>
        public void ShowLogInConsole(string testName, double executionTime, long memoryUsage, bool passed)
        {
            Console.WriteLine(String.Format("Тест: {0} | Статус: {1} | Время исполнения: {2} мс | Объем использованной памяти {3} Мб |\n",
                                            testName, ((passed)?"пройден":"не пройден"),executionTime, memoryUsage));
        }
    }
}
