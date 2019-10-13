using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BayesSpamFilter
{
    public class SpamChecker
    {
        public Dictionary<string, WordProbabilityInfo> WordInfoDictionary { get; }

        public SpamChecker(Dictionary<string, WordProbabilityInfo> wordInfoDictionary)
        {
            WordInfoDictionary = wordInfoDictionary;
        }

        public double GetSpamProbability(string filePath)
        {
            if (File.Exists(filePath))
            {
                var probabilitySum = 0d;
                var allLines = File.ReadAllLines(filePath);
                var words = allLines.Select(l => l.ToLowerInvariant().Split(' ')).SelectMany(w => w).Distinct().ToList();
                var wordCount = 0;
                foreach (var word in words)
                {
                    if (!string.IsNullOrEmpty(word))
                    {
                        if (WordInfoDictionary.Keys.Contains(word))
                        {
                            wordCount++;
                            var wordProbability = WordInfoDictionary[word].SpamProbability /
                                                  (WordInfoDictionary[word].SpamProbability + WordInfoDictionary[word].HamProbability);
                            probabilitySum += wordProbability;
                        }
                    }
                }

                return probabilitySum / wordCount;
            }
            throw new FileNotFoundException();
        }
    }
}