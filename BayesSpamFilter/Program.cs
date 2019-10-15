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
        private string hamCalibrationPath;
        private string spamCalibrationPath;
        private string hamTestPath;
        private string spamTestPath;

        private int hamFileCount;
        private int spamFileCount;
        private Dictionary<string, WordProbabilityInfo> wordInfoDictionary;

        // Alpha is used for words which are only in spam/ ham emails
        private double alpha = 0.000001;
        // The threshold is initially set to 0.5, because a mail is either spam or ham
        // This value will be calibrated during execution
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

        /// <summary>
        /// The threshold will be adjusted by +0.0025 until:
        /// The ratio for all wrongly marked spam files >= the ratio for all wrongly marked ham files
        /// </summary>
        /// <param name="hamCalibrationPath"> Path to the ham calibration folder. </param>
        /// <param name="spamCalibrationPath"> Path to the spam calibration folder. </param>
        private void CalibrateThreshold(string hamCalibrationPath, string spamCalibrationPath)
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


                do
                {
                    hamMarkedAsSpam = GetHamMarkedAsSpamCounter(hamFilePaths, spamChecker);
                    spamMarkedAsHam = GetSpamMarkedAsHamCounter(spamFilePaths, spamChecker);

                    hamMarkedAsSpamRatio = (double)hamMarkedAsSpam / hamFilePaths.Length;
                    spamMarkedAsHamRatio = (double)spamMarkedAsHam / spamFilePaths.Length;

                    // Threshold is adjusted as long as our ratio for wrongly marked ham files is higher the the wrongly marked spam files
                    threshold += 0.0025;

                } while (hamMarkedAsSpamRatio >= spamMarkedAsHamRatio); // Both error ratios should be the same.

                Console.WriteLine($"Optimal threshold is {threshold}.");
                Console.WriteLine($"Threshold calculated using alpha: {alpha}.");
                Console.WriteLine($"{hamMarkedAsSpam} Ham Mails of totally {hamFilePaths.Length} where marked as Spam.");
                Console.WriteLine($"{spamMarkedAsHam} Spam Mails of totally {spamFilePaths.Length} where marked as Ham.");
                Console.WriteLine($"Ham Error Ratio:  {Math.Round(hamMarkedAsSpamRatio * 100, 4, MidpointRounding.ToEven)}%.");
                Console.WriteLine($"Spam Error Ratio: {Math.Round(spamMarkedAsHamRatio * 100, 4, MidpointRounding.ToEven)}%.");
            }
            else
            {
                throw new DirectoryNotFoundException();
            }
        }

        /// <summary>
        /// Counts all wrongly marked ham files
        /// </summary>
        /// <param name="hamFilePaths"> Path to ham files. </param>
        /// <param name="spamChecker"> The checker to use. </param>
        /// <returns></returns>
        private int GetHamMarkedAsSpamCounter(string[] hamFilePaths, SpamChecker spamChecker)
        {
            var hamMarkedAsSpam = 0;
            foreach (var hamFilePath in hamFilePaths)
            {
                var spamProbability = spamChecker.GetSpamProbability(hamFilePath);
                if (spamProbability >= threshold) hamMarkedAsSpam++;

            }
            return hamMarkedAsSpam;
        }

        /// <summary>
        /// Count all wrongly marked spam files.
        /// </summary>
        /// <param name="spamFilePaths"> Path to spam files. </param>
        /// <param name="spamChecker"> The checker to use. </param>
        /// <returns></returns>
        private int GetSpamMarkedAsHamCounter(string[] spamFilePaths, SpamChecker spamChecker)
        {
            var spamMarkedAsHam = 0;
            foreach (var spamFilePath in spamFilePaths)
            {
                var spamProbability = spamChecker.GetSpamProbability(spamFilePath);
                if (spamProbability < threshold) spamMarkedAsHam++;
            }
            return spamMarkedAsHam;
        }

        /// <summary>
        /// This method runs our bayes spam filter after the threshold was calibrated
        /// and prints the results
        /// </summary>
        /// <param name="folderPath"> Path to the email folder. </param>
        private void RunBayesSpamFilter(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                var isInSpamFolder = folderPath.Equals(spamTestPath);
                List<double> errors = new List<double>();

                var filePaths = Directory.GetFiles(folderPath);
                var spamChecker = new SpamChecker(wordInfoDictionary);

                // The spam probabilities of each file are calculated and all the errors are added to a separate list
                foreach (var filePath in filePaths)
                {
                    var spamProbability = spamChecker.GetSpamProbability(filePath);

                    if (isInSpamFolder && spamProbability <= threshold)
                    {
                        errors.Add(spamProbability);// These should be spam, but aren't marked by our filter
                    }
                    else if (!isInSpamFolder && spamProbability >= threshold)
                    {
                        errors.Add(spamProbability);// These should be ham, but aren't marked by our filter
                    }
                }                
                PrintTestResult(folderPath, isInSpamFolder, errors, filePaths);
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
            Console.WriteLine($"Error Ratio is {Math.Round((double)errors.Count / filePaths.Length * 100, 4, MidpointRounding.ToEven)}%.");
        }

        /// <summary>
        /// This method builds a dictionary (equivalent of map in java) with each distinct word and their probabilities to be either spam or ham
        /// The source of our calculations are already classified as ham or spam mails
        /// </summary>
        private void BuildWordInfoDictionary()
        {
            var hamWordCounter = new WordProbabilityInitializer(hamLeanPath);
            var hamCountByWord = hamWordCounter.GetWordCount();
            hamFileCount = hamWordCounter.FileCount;
            var spamWordCounter = new WordProbabilityInitializer(spamLearnPath);
            var spamCountByWord = spamWordCounter.GetWordCount();
            spamFileCount = spamWordCounter.FileCount;
            wordInfoDictionary = GetHamAndSpamCountDictionary(hamCountByWord, spamCountByWord);

            Console.WriteLine($"Generated word probability dictionary with {wordInfoDictionary.Count} words.");
        }

        /// <summary>
        /// All spam and ham emails have a counter for each distinct word.
        /// This method merges the two dictionaries to one and adds the probabilities in a new POCO class
        /// </summary>
        /// <param name="hamCountByWord"></param>
        /// <param name="spamCountByWord"></param>
        /// <returns></returns>
        private Dictionary<string, WordProbabilityInfo> GetHamAndSpamCountDictionary(Dictionary<string, int> hamCountByWord, Dictionary<string, int> spamCountByWord)
        {
            var hamAndSpamCountDictionary = new Dictionary<string, WordProbabilityInfo>();
            // All words from both dictionaries are used
            var allWords = hamCountByWord.Keys.Concat(spamCountByWord.Keys).Distinct().ToList();

            foreach (var word in allWords)
            {
                // If a word is only in the ham files alpha will be used for the spam word count and vice versa
                var hamCount = hamCountByWord.ContainsKey(word) ? hamCountByWord[word] : alpha;
                var spamCount = spamCountByWord.ContainsKey(word) ? spamCountByWord[word] : alpha;

                // Both probabilities are added into the new dictionary using a POCO class
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
            Console.WriteLine("Calculation finished.");
            Console.WriteLine("Press any key to close.");
            Console.WriteLine("------------------------------------------------");
            Console.ReadKey();
        }

        /// <summary>
        /// Gets relativ path to the input files in this project
        /// </summary>
        /// <param name="directoryPath"> Path to the directory inside input files. </param>
        /// <returns> The Path to the Directory. </returns>
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
