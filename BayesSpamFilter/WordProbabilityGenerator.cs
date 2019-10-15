using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BayesSpamFilter
{
    public class WordProbabilityGenerator
    {
        private readonly string inputPath;
        public int FileCount { get; private set; }

        public WordProbabilityGenerator(string inputPath)
        {
            this.inputPath = inputPath;
        }

        public Dictionary<string, int> GetWordCount()
        {
            var wordCountDictionary = new Dictionary<string, int>();

            if (Directory.Exists(inputPath))
            {
                var filePaths = Directory.GetFiles(inputPath);

                if (filePaths.Any())
                {
                    FileCount = filePaths.Length;

                    //increments word counter for each distinct word if found
                    foreach (var filePath in filePaths)
                    {                        
                        foreach (var word in SpamChecker.GetDistinctWordsOfFile(filePath))
                        {                            
                            if (!string.IsNullOrEmpty(word))
                            {
                                if (wordCountDictionary.ContainsKey(word))
                                    wordCountDictionary[word]++;
                                else                                
                                    wordCountDictionary.Add(word, 1);                                
                            }
                        }
                    }
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
            else
            {
                throw new DirectoryNotFoundException();
            }

            return wordCountDictionary;
        }
    }
}
