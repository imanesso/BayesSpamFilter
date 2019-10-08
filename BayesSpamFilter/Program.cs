using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace BayesSpamFilter
{
    public class Program
    {
        private string hamLeanPath;
        private string spamLearnPath;
        private string hamTestPath;
        private string spamTestPath;
        private int hamFileCount;
        private int spamFileCount;
        private decimal defaultCount = 0.8m;

        static void Main(string[] args)
        {
            var p = new Program();
            p.Execute();
        }

        private void Execute()
        {
            PrintHeader();
            InitFilePaths();

            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Started the learning phase.");
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();

            var hamWordCounter = new WordCounter(hamLeanPath);
            var hamCountByWord = hamWordCounter.GetWordCount();
            hamFileCount = hamWordCounter.FileCount;
            var spamWordCounter = new WordCounter(spamLearnPath);
            var spamCountByWord = spamWordCounter.GetWordCount();
            spamFileCount = spamWordCounter.FileCount;
            var wordInfoDictionary = GetHamAndSpamCountDictionary(hamCountByWord, spamCountByWord);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Finished the learning phase.");
            Console.WriteLine("------------------------------------------------");

            var filePaths = Directory.GetFiles(hamTestPath);
            var spamChecker = new SpamChecker(wordInfoDictionary);
            foreach (var filePath in filePaths)
            {
                bool isSpam = spamChecker.IsSpam(filePath, spamFileCount, spamCountByWord, hamFileCount, hamCountByWord);

                string text = isSpam? filePath.Split("\\").Last() +" is probably Spam"
                    : filePath.Split("\\").Last() + " is probably not Spam";

                Console.WriteLine(text);
            }


            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Started the testing phase.");
            Console.WriteLine("------------------------------------------------");



            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Finished the testing phase.");
            Console.WriteLine("------------------------------------------------");


            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Started the testing phase.");
            Console.WriteLine("------------------------------------------------");

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Finished the testing phase.");
            Console.WriteLine("------------------------------------------------");

            PrintFooter();
        }

        private Dictionary<string, WordInfo> GetHamAndSpamCountDictionary(Dictionary<string, int> hamCountByWord, Dictionary<string, int> spamCountByWord)
        {
            var hamAndSpamCountDictionary = new Dictionary<string, WordInfo>();

            var allWords = hamCountByWord.Keys.Concat(spamCountByWord.Keys).Distinct().ToList();

            foreach (var word in allWords)
            {
                var hamCount = hamCountByWord.ContainsKey(word) ? hamCountByWord[word] : defaultCount;
                var spamCount = spamCountByWord.ContainsKey(word) ? spamCountByWord[word] : defaultCount;
                var hamProbability = hamCount / hamFileCount;
                var spamProbability = spamCount / spamFileCount;
                hamAndSpamCountDictionary.Add(word, new WordInfo(hamProbability, spamProbability));
            }

            return hamAndSpamCountDictionary;
        }

        private void InitFilePaths()
        {
            Console.WriteLine("Ham Learn Directory Path:");
            var inputLearnHamPath = Console.ReadLine();
            hamLeanPath = string.IsNullOrWhiteSpace(inputLearnHamPath)
                ? GetRelativePath("ham-anlern")
                : inputLearnHamPath;
            Console.WriteLine();

            Console.WriteLine("Spam Learn Directory Path:");
            var inputLearnSpamPath = Console.ReadLine();
            spamLearnPath = string.IsNullOrWhiteSpace(inputLearnSpamPath)
                ? GetRelativePath("spam-anlern")
                : inputLearnSpamPath;
            Console.WriteLine();

            Console.WriteLine("Ham Test Directory Path:");
            var inputTestHamPath = Console.ReadLine();
            hamTestPath = string.IsNullOrWhiteSpace(inputTestHamPath)
                ? GetRelativePath("ham-test")
                : inputTestHamPath;
            Console.WriteLine();


            Console.WriteLine("Spam Test Directory Path:");
            var inputTestSpamPath = Console.ReadLine();
            spamTestPath = string.IsNullOrWhiteSpace(inputTestSpamPath)
                ? GetRelativePath("spam-test")
                : inputTestHamPath;
            Console.WriteLine();
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

        private static string GetRelativePath(string directoryPath)
        {
            var projectFolder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            var file = Path.Combine(projectFolder, "InputFiles",directoryPath);

            return file;
        }
    }
}
