using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BayesSpamFilter
{
    public class SpamChecker
    {
        public Dictionary<string, WordInfo> WordInfoDictionary { get; }

        public SpamChecker(Dictionary<string, WordInfo> wordInfoDictionary)
        {
            WordInfoDictionary = wordInfoDictionary;
        }

        public decimal CalculateSpamProbability(string filePath)
        {
            var probability = 1.0m;
            if (File.Exists(filePath))
            {
                var allLines = File.ReadAllLines(filePath);
                var words = allLines.Select(l => l.ToLowerInvariant().Split(' ')).SelectMany(w => w).Distinct().ToList();

                foreach (var word in words)
                {
                    if (WordInfoDictionary.Keys.Contains(word))
                    {
                        probability *= WordInfoDictionary[word].SpamProbability /
                                       (WordInfoDictionary[word].SpamProbability +
                                        WordInfoDictionary[word].HamProbability);
                    }
                }
            }
            else
            {
                throw new FileNotFoundException($"File '{filePath}' was not found.");
            }

            return probability;
        }
    }
}