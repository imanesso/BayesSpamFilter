using System;
using System.Collections.Generic;
using System.Linq;

namespace BayesSpamFilter
{
    public class Program
    {
        private string hamPath;
        private string spamPath;
        private decimal defaultCount = 0.01m;

        static void Main(string[] args)
        {
            var p = new Program();
            p.Execute();
        }

        private void Execute()
        {
            PrintHeader();
            InitFilePaths();

            var hamWordCounter = new WordCounter(hamPath);
            var hamWordCountDictionary = hamWordCounter.GetWordCount();

            var spamWordCounter = new WordCounter(spamPath);
            var spamWordCountDictionary = spamWordCounter.GetWordCount();

            var hamAndSpamCount = GetHamAndSpamCountDictionary(hamWordCountDictionary, spamWordCountDictionary);


            PrintFooter();
        }

        private Dictionary<string, CounterInfo> GetHamAndSpamCountDictionary(Dictionary<string, int> hamWordCountDictionary, Dictionary<string, int> spamWordCountDictionary)
        {
            var hamAndSpamCountDictionary = new Dictionary<string, CounterInfo>();

            var allWords = hamWordCountDictionary.Keys.Concat(spamWordCountDictionary.Keys).Distinct().ToList();

            foreach (var word in allWords)
            {
                var hamCount = hamWordCountDictionary.ContainsKey(word) ? hamWordCountDictionary[word] : defaultCount;
                var spamCount = spamWordCountDictionary.ContainsKey(word) ? spamWordCountDictionary[word] : defaultCount;
                hamAndSpamCountDictionary.Add(word, new CounterInfo(hamCount, spamCount));
            }

            return hamAndSpamCountDictionary;
        }

        private void InitFilePaths()
        {
            Console.WriteLine("Ham Directory Path:");
            var inputHamPath = Console.ReadLine();
            hamPath = string.IsNullOrWhiteSpace(inputHamPath)
                ? @"C:\git\dist\BayesSpamFilter\Mails\Learning\Ham"
                : inputHamPath;

            Console.WriteLine("Spam Directory Path:");
            var inputSpamPath = Console.ReadLine();
            spamPath = string.IsNullOrWhiteSpace(inputSpamPath)
                ? @"C:\git\dist\BayesSpamFilter\Mails\Learning\Spam"
                : inputSpamPath;
        }

        private static void PrintHeader()
        {
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Diskrete Stochastik");
            Console.WriteLine("Bayes-Spam-Filter");
            Console.WriteLine("Crated by Anessollah Ima & Jonathan Bättig");
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void PrintFooter()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Calculation finished");
            Console.WriteLine("------------------------------------------------");
            Console.ReadKey();
        }
    }
}
