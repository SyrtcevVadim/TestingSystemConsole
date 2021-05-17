using System;
using System.IO;
using System.Diagnostics;

namespace TestingSystemConsole
{
    struct TestResult
    {
        /// <summary>
        /// Название теста
        /// </summary>
        public string TestName { get; set; }
        /// <summary>
        /// Статус теста
        /// </summary>
        public bool IsPassed { get; set; }
        /// <summary>
        /// Среднее время работы алгоритма для данного теста
        /// </summary>
        public double AverageElapsedTime { get; set; }
        /// <summary>
        /// Количество использованной памяти(в байтах)
        /// </summary>
        public double MemoryUsage { get; set; }

    };

    /// <summary>
    /// Класс, предназначенный для тестирования пользовательской программы
    /// </summary>
    class Tester
    {
        /// <summary>
        /// Хранит результаты тестирования
        /// </summary>
        private TestResult[] results;

        /// <summary>
        /// Количество итераций для работы одного теста
        /// </summary>
        private int testIterations=15;
        /// <summary>
        /// Процесс, в котором работает пользовательская программа
        /// </summary>
        private Process userExecutable;

        /// <summary>
        /// Читает информацию из файла тестов
        /// </summary>
        private TestReader testReader;

        /// <summary>
        /// Читает информацию из файлов ответов
        /// </summary>
        private AnwerReader answersReader;

        /// <summary>
        /// Читает информацию из файлов ограничений
        /// </summary>
        private RestrictionReader restrictionsReader;

        /// <summary>
        /// Поток, связанный с файлом результатов тестирования
        /// </summary>
        private FileStream logFile;

        /// <param name="pathToUserExecutableFile">Путь к пользовательской программе</param>
        /// <param name="pathToTestsFile">Путь к файлу тестов</param>
        /// <param name="pathToAnswersFile">Путь к файлу ответов</param>
        /// <param name="pathToRestrictionsFile">Путь к файлу с ограничениями</param>
        /// <param name="pathToLogFile">Путь к файлу результатов тестирования</param>
        public Tester(
                        string pathToUserExecutableFile, 
                        string pathToTestsFile, 
                        string pathToAnswersFile,
                        string pathToRestrictionsFile)
        {
            // Создаем новый процесс для тестирования пользовательской программы
            userExecutable = new Process();
            // Настраиваем поток для работы с пользовательской программой
            ConfigureProcess(pathToUserExecutableFile);
            // Создаем файл для сохранения результатов тестирования
            MakeLogFile();
            
            // Связываем файлы с объектами для их чтения
            testReader = new TestReader(pathToTestsFile);
            answersReader = new AnwerReader(pathToAnswersFile);
            restrictionsReader = new RestrictionReader(pathToRestrictionsFile);
        }

        ~Tester()
        {
            // Освобождаем ресурсы, отведённые для потока
            userExecutable.Dispose();
        }

        public void MakeLogFile(string pathToLogFile= @"C:/Projects/TestingSystemConsole/TestData/log.txt")
        {
            try
            {
                logFile = new FileStream(pathToLogFile, FileMode.Truncate);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Настраивает поток для тестирования пользовательской программы
        /// </summary>
        /// <param name="pathToUserExecutableFile">Путь к пользовательской программе</param>
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
                // Это нужно для подачи в пользовательскую программу входных данных 
                // и для получения вывода программы после тестирования
                userExecutable.StartInfo.RedirectStandardInput = true;
                userExecutable.StartInfo.RedirectStandardOutput = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        /// <summary>
        /// Начинает тестирование пользовательской программы
        /// </summary>
        public void Start()
        {
            // Получаем количество тестовых случаев
            int testQuantity = testReader.TestQuantity;
            // Создаем массив результатов тестирования
            results = new TestResult[testQuantity];
            for (int counter = 0; counter < testQuantity; counter++)
            {
                // Изначально мы считаем, что тест пройден
                results[counter].IsPassed = true;
                // Получаем тестовые данные
                string currentTest = testReader.GetNextTestData();
                // Записываем название текущего теста
                results[counter].TestName = testReader.CurrentTestName;

                // Среднее арифметическое времени работы программы на текущем тесте
                double workingTimeMean = 0.0;
                for (int i = 0; i < testIterations; i++)
                {
                    
                    if (userExecutable.Start())
                    {
                        // Устанавливаем высокий приоритет исполнения для данного потока чтобы сократить побочное влияние других программ
                        userExecutable.PriorityClass = ProcessPriorityClass.High;

                        // Получаем стандартный поток ввода/вывода тестируемой программы
                        StreamWriter processInput = userExecutable.StandardInput;
                        StreamReader processOutput = userExecutable.StandardOutput;

                        // Вводим в тестируемую программу данные
                        processInput.Write(currentTest);

                        // Замеряем время работы программы
                        Stopwatch timer = new Stopwatch();
                        timer.Start();
                        // Ожидаем завершения работы тестируемой программы
                        while (!userExecutable.HasExited)
                        {
                            // Если алгоритм превысил допустимое время работы
                            if (timer.ElapsedMilliseconds > restrictionsReader.TimeLimitInMilliseconds)
                            {
                                userExecutable.Kill();
                                timer.Stop();
                                results[counter].IsPassed = false;
                                results[counter].MemoryUsage = userExecutable.WorkingSet64;
                                results[counter].AverageElapsedTime = restrictionsReader.TimeLimitInMilliseconds;
                                break;
                            }
                            results[counter].MemoryUsage = userExecutable.WorkingSet64;
                        }
                        timer.Stop();
                        
                        // Проверяем удовлетворение ограничениям
                        if (results[counter].IsPassed)
                        {
                            workingTimeMean += timer.ElapsedMilliseconds;
                        }
                        else
                        {
                            break;
                        }

                        // Для первой итерации будем проверять корректность работы программы
                        if (i == 0)
                        {
                            // Получаем от программы данные выходного потока
                            string output = processOutput.ReadToEnd();
                            // Проверяем выходные данные на корректность
                            results[counter].IsPassed = answersReader.IsAnswerCorrect(output);
                            
                        }     
                    }
                    else
                    {
                        throw new Exception("Процесс не был создан. Тест:" + counter); 
                    }
                }
                // Подсчитываем среднее время работы программы для текущего теста
                workingTimeMean /= testIterations;
                results[counter].AverageElapsedTime = workingTimeMean;

                // Пишем результат тестирования в файл
                SaveLogToFile(results[counter]);
            }
        }

        /// <summary>
        /// Сохраняет результат прохождения теста в выходной файл
        /// </summary>
        /// <param name="testName">Название теста</param>
        /// <param name="executionTime">Время, затраченное программой на выполнение теста</param>
        /// <param name="memoryUsage">Количество используемой программой памяти в мегабайтах</param>
        /// <param name="passed">Показывает, пройден программой данный тест или нет</param>
        public void SaveLogToFile(TestResult result)
        {
            StreamWriter logStream = new StreamWriter(logFile);
            logStream.WriteLine(String.Format("Тест: {0} | Статус: {1} | Время исполнения: {2:#.##} мс | Объем использованной памяти {3}б |\n",
                                            result.TestName, 
                                            (result.IsPassed) ? "пройден" : "не пройден", 
                                            result.AverageElapsedTime, result.MemoryUsage));
            logStream.Flush();
        }

        /// <summary>
        /// Отображает результаты тестирования в консоли
        /// </summary>
        /// <param name="testName">Название теста</param>
        /// <param name="executionTime">Время, затраченное программой на выполнение теста</param>
        /// <param name="memoryUsage">Количество используемой программой памяти в мегабайтах</param>
        /// <param name="passed">Показывае, пройден программой данный тест или нет</param>
        public void ShowResultsInConsole()
        {
            for (int i = 0; i < results.Length; i++)
            {
                
                if(results[i].IsPassed)
                {
                    // Если тест пройден, выводим сообщение о нём зеленым цветом
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    // Если тест не пройден, выводим сообщение о нем красным цветом
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.WriteLine(String.Format("Тест: {0,-6} | Статус: {1,-10} | Время исполнения: {2:#.##} мс | Объем использованной памяти {3,-5} б |\n",
                                            results[i].TestName, (results[i].IsPassed) ? "пройден" : "не пройден",
                                            results[i].AverageElapsedTime,
                                            results[i].MemoryUsage));
                
            }
            // Возвращаем консоли нормальный цвет
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
