using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BayesSpamFilter
{
    public class WordCounter
    {
        private readonly string inputPath;

        public WordCounter(string inputPath)
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
                    foreach (var filePath in filePaths)
                    {
                        var allLines = File.ReadAllLines(filePath);

                        foreach (var line in allLines)
                        {
                            var words = line.ToLowerInvariant().Split(' ');

                            foreach (var word in words)
                            {
                                if (wordCountDictionary.ContainsKey(word))
                                {
                                    wordCountDictionary[word]++;
                                }
                                else
                                {
                                    wordCountDictionary.Add(word, 1);
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new FileNotFoundException($"The input directory '{inputPath}' is empty.");
                }
            }
            else
            {
                throw new DirectoryNotFoundException($"The input directory '{inputPath}' not found.");
            }

            return wordCountDictionary;
        }
    }
}
