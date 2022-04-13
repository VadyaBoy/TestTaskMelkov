using System;
using System.Threading;
using System.Diagnostics;

namespace Melkov_Frequency_Analysis
{
    internal class Program
    {
        async static Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Введите путь к файлу:");
                // Список тасков для многопоточного считывания файла
                List<Task<List<string>>> tasks = new List<Task<List<string>>>();
                // Объект класса для измерения времени работы программы
                Stopwatch stopWatch = new Stopwatch();
                // Считываем файл ассинхронно
                using (StreamReader sr = new StreamReader(Console.ReadLine()))
                {
                    stopWatch.Start();
                    // Формируем алфавит триплетов с русскими буквами
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        tasks.Add(FrequencyAnalysis(line));
                    }
                }
                // Ожидаем все таски
                await Task.WhenAll(tasks);
                if (tasks.Count > 0)
                {
                    // Получаем список всех триплетов внутри текстового файла
                    List<string> matchTriplets = new List<string>();
                    for (int i = 0; i < tasks.Count; i++)
                    {
                        matchTriplets.AddRange(tasks[i].Result);
                    }
                    // Алфавит триплетов
                    char[] alphabetRU;
                    List<string> triplets = new List<string>();
                    alphabetRU = Enumerable.Range('а', 32).Select(x => (char)x).ToArray();
                    triplets = AddTriplets(alphabetRU);
                    // Список тасков для многопоточного подсчета частоты искомых триплетов
                    List<Task<int>> tasks1 = new List<Task<int>>();
                    for (int i = 0; i < triplets.Count; i++)
                    {
                        tasks1.Add(TripletFrequency(triplets[i], matchTriplets));
                    }
                    // Ожидаем все таски
                    await Task.WhenAll(tasks1);
                    // Заполняем массив частот, соответвующий списку искомых триплетов
                    int[] tripletFrequency = new int[triplets.Count];
                    for (int i = 0; i < tasks1.Count; i++)
                    {
                        tripletFrequency[i] = tasks1[i].Result;
                    }
                    // Выводим топ-10 частот
                    Console.WriteLine(Top10Triplets(tripletFrequency, triplets));
                }
                // Выводим время работы программы
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{00000000:00}", ts.Hours * 3600000 + ts.Minutes * 60000 + ts.Seconds * 1000 + ts.Milliseconds);
                Console.WriteLine("Прошло миллисекунд: " + elapsedTime);
            }
            catch (Exception e)
            {
                Console.WriteLine("Файл не может быть прочитан:");
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
            // Метод поиска триплетов в текстовой строке
            async Task<List<string>> FrequencyAnalysis(string textRow)
            {
                await Task.Delay(1);
                // Выходной список триплетов из входного текста
                List<string> tripletsFromText = new List<string>();
                // Разбиение строки на слова
                string[] textFromInputString = textRow.Split(new char[] { ' ', '.', ',', '!', '?', '-', '"', '—', ':', ';' }, StringSplitOptions.RemoveEmptyEntries);
                // Триплет
                string triplet;
                // Проходимся по выходному списку всех комбинаций 
                foreach (string letters in textFromInputString)
                {
                    for (int i = 0; i < letters.Length; i++)
                    {
                        triplet = string.Join("", letters.ToLower().ToString().Skip(i).Take(3));
                        // Сравниваем комбинацию символов строки с комбинацие из списка групп
                        if (triplet != null && triplet.Length > 2)
                        {
                            // Добавляем в выходной список данные
                            tripletsFromText.Add(triplet);
                        }
                    }
                }
                return tripletsFromText;
            }
            // Метод для формирования алфавита триплетов
            List<string> AddTriplets(char[] alphabet)
            {
                List<string> triplets = new List<string>();
                for (int i = 0; i < alphabet.Length; i++)
                {
                    for (int j = 0; j < alphabet.Length; j++)
                    {
                        for (int k = 0; k < alphabet.Length; k++)
                        {
                            triplets.Add(alphabet[i].ToString() + alphabet[j].ToString() + alphabet[k].ToString());
                        }
                    }
                }
                return triplets;
            }
            // Метод подсчета частот искомых триплетов
            async Task<int> TripletFrequency(string triplet, List<string> matchTriplets)
            {
                await Task.Delay(1);
                int count = 0;
                for (int i = 0; i < matchTriplets.Count; i++)
                {
                    if (triplet == matchTriplets[i])
                    {
                        count++;
                    }
                }
                return count;
            }
            // Метод вывода топ-10 триплетов
            string Top10Triplets(int[] tripletsFrequency, List<string> triplets)
            {
                int[] tripletsFrequencyLocal = tripletsFrequency;
                string top10Triplets = "";
                int max;
                int maxId;
                for (int i = 0; i < 10; i++)
                {
                    max = 0;
                    maxId = 0;
                    for (int j = 0; j < tripletsFrequency.Length; j++)
                    {
                        if (max < tripletsFrequency[j])
                        {
                            max = tripletsFrequency[j];
                            maxId = j;
                        }
                    }
                    if (max != 0)
                    {
                        top10Triplets += (i + 1).ToString() + ") " + triplets[maxId] + ": " + max + "\r\n";
                        tripletsFrequency[maxId] = 0;
                    }
                }
                return top10Triplets;
            }
        }
    }
}
