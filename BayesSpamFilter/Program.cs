using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace BayesSpamFilter
{
    public class Program
    {
        private string hamLeanPath;
        private string spamLearnPath;
        private string hamCalibrationPath;
        private string spamCalibrationPath;
        private string hamTestPath;
        private string spamTestPath;

        private int hamFileCount;
        private int spamFileCount;

        private int minimumWordOccurrence = 3;
        private double alpha = 0.000000000000000000000000000000001;
        private double threshold = 0.9;

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

            var wordInfoDictionary = BuildWordInfoDictionary();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Finished the learning phase.");
            Console.WriteLine("------------------------------------------------");


            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Started the calibration phase.");
            Console.WriteLine("------------------------------------------------");

            CalibrateThreshold(hamCalibrationPath, spamCalibrationPath, wordInfoDictionary);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Finished the calibration phase.");
            Console.WriteLine("------------------------------------------------");


            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Started the testing phase.");
            Console.WriteLine("------------------------------------------------");

            RunBayesSpamFilter(hamTestPath, wordInfoDictionary);
            RunBayesSpamFilter(spamTestPath, wordInfoDictionary);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Finished the testing phase.");
            Console.WriteLine("------------------------------------------------");

            PrintFooter();
        }

        private void CalibrateThreshold(string hamCalibrationPath, string spamCalibrationPath, Dictionary<string, WordInfo> wordInfoDictionary)
        {
            if (Directory.Exists(hamCalibrationPath) && Directory.Exists(spamCalibrationPath))
            {
                var spamChecker = new SpamChecker(wordInfoDictionary);
                var hamFilePaths = Directory.GetFiles(hamCalibrationPath);
                var spamFilePaths = Directory.GetFiles(spamCalibrationPath);
                var hamMarkedAsSpam = 0;
                var spamMarkedAsHam = 0;
                var hamMarkedAsSpamRatio = 0d;
                var spamMarkedAsHamRatio = 0d;


                while (hamMarkedAsSpamRatio <= spamMarkedAsHamRatio)
                {
                    List<double> hamList = new List<double>();
                    foreach (var hamFilePath in hamFilePaths)
                    {
                        var spamProbability = spamChecker.CalculateSpamProbability(hamFilePath);
                        if (spamProbability >= threshold)
                        {
                            hamMarkedAsSpam++;
                        }
                    }

                    foreach (var spamFilePath in spamFilePaths)
                    {
                        var spamProbability = spamChecker.CalculateSpamProbability(spamFilePath);
                        if (spamProbability < threshold)
                        {
                            spamMarkedAsHam++;
                        }
                    }

                    hamMarkedAsSpamRatio = (double)hamMarkedAsSpam / hamFilePaths.Length;
                    spamMarkedAsHamRatio = (double)spamMarkedAsHam / spamFilePaths.Length;

                    if (hamMarkedAsSpamRatio <= spamMarkedAsHamRatio)
                    {
                        threshold += 0.01;
                    }

                    hamMarkedAsSpam = 0;
                    spamMarkedAsHam = 0;
                }
            }
            else
            {
                throw new DirectoryNotFoundException();
            }
        }

        private void RunBayesSpamFilter(string folderPath, Dictionary<string, WordInfo> wordInfoDictionary)
        {
            if (Directory.Exists(folderPath))
            {
                var filePaths = Directory.GetFiles(folderPath);
                var spamChecker = new SpamChecker(wordInfoDictionary);
                foreach (var filePath in filePaths)
                {
                    var d = spamChecker.CalculateSpamProbability(filePath);
                }
            }
            else
            {
                throw new DirectoryNotFoundException();
            }
        }

        private Dictionary<string, WordInfo> BuildWordInfoDictionary()
        {
            var hamWordCounter = new WordCounter(hamLeanPath);
            var hamCountByWord = hamWordCounter.GetWordCount();
            hamFileCount = hamWordCounter.FileCount;
            var spamWordCounter = new WordCounter(spamLearnPath);
            var spamCountByWord = spamWordCounter.GetWordCount();
            spamFileCount = spamWordCounter.FileCount;
            var wordInfoDictionary = GetHamAndSpamCountDictionary(hamCountByWord, spamCountByWord);
            return wordInfoDictionary;
        }

        private Dictionary<string, WordInfo> GetHamAndSpamCountDictionary(Dictionary<string, int> hamCountByWord, Dictionary<string, int> spamCountByWord)
        {
            var hamAndSpamCountDictionary = new Dictionary<string, WordInfo>();

            var allWords = hamCountByWord.Keys.Concat(spamCountByWord.Keys).Distinct().ToList();

            foreach (var word in allWords)
            {
                var hamCount = hamCountByWord.ContainsKey(word) ? hamCountByWord[word] : alpha;
                var spamCount = spamCountByWord.ContainsKey(word) ? spamCountByWord[word] : alpha;
                var hamProbability =  hamCount / hamFileCount;
                var spamProbability = spamCount / spamFileCount;
                hamAndSpamCountDictionary.Add(word, new WordInfo(hamProbability, spamProbability));
            }

            return hamAndSpamCountDictionary;
        }

        private void InitFilePaths()
        {
            hamLeanPath = GetRelativePath("ham-anlern");
            Console.WriteLine($"Ham Learn Directory Path: {hamLeanPath}");
            Console.WriteLine();

            spamLearnPath = GetRelativePath("spam-anlern");
            Console.WriteLine($"Spam Learn Directory Path: {spamLearnPath}");
            Console.WriteLine();

            hamCalibrationPath = GetRelativePath("ham-kalibrierung");
            Console.WriteLine($"Ham Calibration Directory Path: {hamTestPath}");
            Console.WriteLine();

            spamCalibrationPath = GetRelativePath("spam-kalibrierung");
            Console.WriteLine($"Spam Calibration Directory Path: {spamTestPath}");
            Console.WriteLine();

            hamTestPath = GetRelativePath("ham-test");
            Console.WriteLine($"Ham Test Directory Path: {hamTestPath}");
            Console.WriteLine();

            spamTestPath = GetRelativePath("spam-test");
            Console.WriteLine($"Spam Test Directory Path: {spamTestPath}");
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
            var directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            if (directoryInfo != null)
            {
                var projectFolder = directoryInfo.Parent.FullName;
                var path = Path.Combine(projectFolder, "InputFiles", directoryPath);

                return path;
            }
            else
            {
                throw new DirectoryNotFoundException();
            }
        }
    }
}
