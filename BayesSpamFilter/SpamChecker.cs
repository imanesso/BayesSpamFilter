using System;
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

        public double CalculateSpamProbability(string filePath)
        {
            if (File.Exists(filePath))
            {
                var n = 0d;
                var allLines = File.ReadAllLines(filePath);
                var words = allLines.Select(l => l.ToLowerInvariant().Split(' ')).SelectMany(w => w).Distinct().ToList();

                foreach (var word in words)
                {
                    if (!string.IsNullOrEmpty(word))
                    {
                        if (WordInfoDictionary.Keys.Contains(word))
                        {
                            n += Math.Log(WordInfoDictionary[word].HamProbability) -
                                 Math.Log(WordInfoDictionary[word].SpamProbability);
                        }
                    }
                }

                return 1 / (1 + Math.Exp(n));
            }
            throw new FileNotFoundException();
        }
    }
}