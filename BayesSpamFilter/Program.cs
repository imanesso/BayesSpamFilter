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
        private Dictionary<string, WordProbabilityInfo> wordInfoDictionary;

        private double alpha = 0.000001;
        private double threshold = 0.5;

        static void Main(string[] args)
        {
            var p = new Program();
            p.Execute();
        }

        private void Execute()
        {
            PrintHeader();
            InitFilePaths();

            PrintPhaseStart("learning");
            BuildWordInfoDictionary();
            PrintPhaseEnd("learning");

            PrintPhaseStart("calibration");
            CalibrateThreshold(hamCalibrationPath, spamCalibrationPath);
            PrintPhaseEnd("calibration");

            PrintPhaseStart("testing");
            RunBayesSpamFilter(hamTestPath);
            Console.WriteLine();
            RunBayesSpamFilter(spamTestPath);
            PrintPhaseEnd("testing");

            PrintFooter();
        }

        private static void PrintPhaseStart(string phaseName)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine($"Started the {phaseName} phase.");
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void PrintPhaseEnd(string phaseName)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine($"Finished the {phaseName} phase.");
            Console.WriteLine("------------------------------------------------");
        }

        private void CalibrateThreshold(string hamCalibrationPath, string spamCalibrationPath)
        {
            if (Directory.Exists(hamCalibrationPath) && Directory.Exists(spamCalibrationPath))
            {
                var spamChecker = new SpamChecker(wordInfoDictionary);
                var hamFilePaths = Directory.GetFiles(hamCalibrationPath);
                var spamFilePaths = Directory.GetFiles(spamCalibrationPath);
                var hamMarkedAsSpam = int.MaxValue;
                var spamMarkedAsHam = int.MaxValue;
                var hamMarkedAsSpamRatio = 0d;
                var spamMarkedAsHamRatio = 0d;


                while (hamMarkedAsSpamRatio >= spamMarkedAsHamRatio)
                {
                    hamMarkedAsSpam = 0;
                    spamMarkedAsHam = 0;

                    foreach (var hamFilePath in hamFilePaths)
                    {
                        var spamProbability = spamChecker.GetSpamProbability(hamFilePath);
                        if (spamProbability >= threshold)
                        {
                            hamMarkedAsSpam++;
                        }
                    }

                    foreach (var spamFilePath in spamFilePaths)
                    {
                        var spamProbability = spamChecker.GetSpamProbability(spamFilePath);
                        if (spamProbability < threshold)
                        {
                            spamMarkedAsHam++;
                        }
                    }

                    hamMarkedAsSpamRatio = (double)hamMarkedAsSpam / hamFilePaths.Length;
                    spamMarkedAsHamRatio = (double)spamMarkedAsHam / spamFilePaths.Length;

                    if (hamMarkedAsSpamRatio >= spamMarkedAsHamRatio)
                    {
                        threshold += 0.0025;
                    }
                    else
                    {
                        Console.WriteLine($"Optimal threshold is {threshold}");
                        Console.WriteLine($"{hamMarkedAsSpam} Ham Mails of totally {hamFilePaths.Length} where marked as Spam.");
                        Console.WriteLine($"{spamMarkedAsHam} Spam Mails of totally {spamFilePaths.Length} where marked as Ham.");
                        Console.WriteLine($"Ham Error Ration:  {hamMarkedAsSpamRatio * 100}%");
                        Console.WriteLine($"Spam Error Ration: {spamMarkedAsHamRatio * 100}%");
                    }
                }
            }
            else
            {
                throw new DirectoryNotFoundException();
            }
        }

        private void RunBayesSpamFilter(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                var shouldBeSpam = folderPath.Contains("spam-test");
                List<double> errors = new List<double>();

                var filePaths = Directory.GetFiles(folderPath);
                var spamChecker = new SpamChecker(wordInfoDictionary);
                foreach (var filePath in filePaths)
                {
                    var spamProbability = spamChecker.GetSpamProbability(filePath);

                    if (shouldBeSpam && spamProbability <= threshold)
                    {
                        errors.Add(spamProbability);
                    }
                    else if (!shouldBeSpam && spamProbability >= threshold)
                    {
                        errors.Add(spamProbability);
                    }
                }

                PrintTestResult(folderPath, shouldBeSpam, errors, filePaths);
            }
            else
            {
                throw new DirectoryNotFoundException();
            }
        }

        private static void PrintTestResult(string folderPath, bool shouldBeSpam, List<double> errors, string[] filePaths)
        {
            Console.WriteLine($"Checked files in folder {folderPath}.");
            Console.WriteLine($"All files should have been marked as {(shouldBeSpam ? "Spam" : "Ham")}.");
            Console.WriteLine($"{errors.Count} of {filePaths.Length} {(shouldBeSpam ? "Spam" : "Ham")} Mails where marked as {(shouldBeSpam ? "Ham" : "Spam")}.");
            Console.WriteLine($"Error Ration is {(double)errors.Count / filePaths.Length * 100}%");
        }

        private void BuildWordInfoDictionary()
        {
            var hamWordCounter = new WordProbabilityGenerator(hamLeanPath);
            var hamCountByWord = hamWordCounter.GetWordCount();
            hamFileCount = hamWordCounter.FileCount;
            var spamWordCounter = new WordProbabilityGenerator(spamLearnPath);
            var spamCountByWord = spamWordCounter.GetWordCount();
            spamFileCount = spamWordCounter.FileCount;
            wordInfoDictionary = GetHamAndSpamCountDictionary(hamCountByWord, spamCountByWord);

            Console.WriteLine($"Generated word probability dictionary with {wordInfoDictionary.Count} words");
        }

        private Dictionary<string, WordProbabilityInfo> GetHamAndSpamCountDictionary(Dictionary<string, int> hamCountByWord, Dictionary<string, int> spamCountByWord)
        {
            var hamAndSpamCountDictionary = new Dictionary<string, WordProbabilityInfo>();

            var allWords = hamCountByWord.Keys.Concat(spamCountByWord.Keys).Distinct().ToList();

            foreach (var word in allWords)
            {
                var hamCount = hamCountByWord.ContainsKey(word) ? hamCountByWord[word] : alpha;
                var spamCount = spamCountByWord.ContainsKey(word) ? spamCountByWord[word] : alpha;
                var hamProbability = hamCount / hamFileCount;
                var spamProbability = spamCount / spamFileCount;
                hamAndSpamCountDictionary.Add(word, new WordProbabilityInfo(hamProbability, spamProbability));
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
            Console.WriteLine($"Ham Calibration Directory Path: {hamCalibrationPath}");
            Console.WriteLine();

            spamCalibrationPath = GetRelativePath("spam-kalibrierung");
            Console.WriteLine($"Spam Calibration Directory Path: {spamCalibrationPath}");
            Console.WriteLine();

            hamTestPath = GetRelativePath("ham-test");
            Console.WriteLine($"Ham Test Directory Path: {hamTestPath}");
            Console.WriteLine();

            spamTestPath = GetRelativePath("spam-test");
            Console.WriteLine($"Spam Test Directory Path: {spamTestPath}");
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
            Console.WriteLine("Press any key to close");
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
