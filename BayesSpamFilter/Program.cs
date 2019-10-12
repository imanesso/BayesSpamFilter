using System;
using System.Collections.Generic;
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
        private string hamCalibrationPath;
        private string spamCalibrationPath;
        private int hamFileCount;
        private int spamFileCount;
        private double threshold = 1;

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
            Console.WriteLine("Starting the learning phase.");
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();

            var hamWordCounter = new WordCounter(hamLeanPath);
            var hamCountByWord = hamWordCounter.GetWordCount();
            hamFileCount = hamWordCounter.FileCount;
            var spamWordCounter = new WordCounter(spamLearnPath);
            var spamCountByWord = spamWordCounter.GetWordCount();
            spamFileCount = spamWordCounter.FileCount;

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Finished the learning phase.");
            Console.WriteLine("------------------------------------------------");


            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Starting the calibration phase.");
            Console.WriteLine("------------------------------------------------");

            while (FindSpamMails(spamCountByWord, hamCountByWord, hamCalibrationPath) > 10)
            {
                threshold += 0.01;
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Finished the calibration phase.");
            Console.WriteLine("------------------------------------------------");


            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Starting the testing phase.");
            Console.WriteLine("------------------------------------------------");

            FindSpamMails(spamCountByWord, hamCountByWord, hamTestPath);

            Console.WriteLine();
            Console.WriteLine();

            FindSpamMails(spamCountByWord, hamCountByWord, spamTestPath);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Finished the testing phase.");
            Console.WriteLine("------------------------------------------------");

            PrintFooter();
        }

        private double FindSpamMails(Dictionary<string, int> spamCountByWord, Dictionary<string, int> hamCountByWord, string testPath)
        {
            List<double> probs = new List<double>();
            var filePaths = Directory.GetFiles(testPath);
            var spamChecker = new SpamChecker();
            var foundHamMails = 0;
            var foundSpamMails = 0;
            foreach (var filePath in filePaths)
            {
                double spamProbability = spamChecker.CalculateSpamProbability(filePath, spamFileCount, spamCountByWord, hamFileCount, hamCountByWord);
                probs.Add(spamProbability);
                string text = spamProbability > threshold
                    ? filePath.Split("\\").Last() + " is probably Spam"
                    : filePath.Split("\\").Last() + " is probably not Spam";

                if (spamProbability > threshold)
                {
                    foundSpamMails++;
                }
                else
                {
                    foundHamMails++;
                }

                Console.WriteLine(text);
            }

            Console.WriteLine();
            Console.WriteLine($"Calculated Spam and Ham Mails for Path {testPath}");
            Console.WriteLine($"Result: Ham = {foundHamMails}; Spam = {foundSpamMails}");
            Console.WriteLine($"Result: Spam % {(double)foundSpamMails / filePaths.Length * 100}.");
            return (double)foundSpamMails / filePaths.Length * 100;
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


            Console.WriteLine("Ham Calibration Directory Path:");
            var inputCalibrationHamPath = Console.ReadLine();
            hamCalibrationPath = string.IsNullOrWhiteSpace(inputCalibrationHamPath)
                ? GetRelativePath("ham-kallibrierung")
                : inputCalibrationHamPath;
            Console.WriteLine();

            Console.WriteLine("Spam Calibration Directory Path:");
            var inputCalibrationSpamPath = Console.ReadLine();
            spamCalibrationPath = string.IsNullOrWhiteSpace(inputCalibrationSpamPath)
                ? GetRelativePath("spam-kallibrierung")
                : inputCalibrationSpamPath;
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
            var file = Path.Combine(projectFolder, "InputFiles", directoryPath);

            return file;
        }
    }
}
