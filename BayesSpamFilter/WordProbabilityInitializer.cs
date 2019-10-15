using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BayesSpamFilter
{
    public class WordProbabilityInitializer
    {
        private readonly string inputPath;
        public int FileCount { get; private set; }

        /// <summary>
        /// Constructor of the WordProbabilityInitializer.
        /// </summary>
        /// <param name="inputPath"> Path to a directory with email messages. </param>
        public WordProbabilityInitializer(string inputPath)
        {
            this.inputPath = inputPath;
        }

        /// <summary>
        /// This Function calculates how many times each word is contained in given messages.
        /// </summary>
        /// <returns> Returns a dictionary with a word as the key and the given word count as the value. </returns>
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
