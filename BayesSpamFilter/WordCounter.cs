using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BayesSpamFilter
{
    public class WordCounter
    {
        private readonly string inputPath;
        public int FileCount { get; private set; }

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
                    FileCount = filePaths.Length;

                    foreach (var filePath in filePaths)
                    {
                        var allLines = File.ReadAllLines(filePath);
                        var words = allLines.Select(l => l.ToLowerInvariant().Split(' ')).SelectMany(w => w).Distinct().ToList();

                        foreach (var word in words)
                        {
                            if (!string.IsNullOrEmpty(word))
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
