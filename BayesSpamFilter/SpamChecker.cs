using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BayesSpamFilter
{
    public class SpamChecker
    {
        //As our normal Calculatioin did not work, we tried the following source:
        //https://netmatze.wordpress.com/2013/05/25/building-a-simple-spam-filter-with-naive-bayes-classifier-and-c/
        public double CalculateSpamProbability(string filePath, int spamFileCount, Dictionary<string, int> spamCountByWord, int hamFileCount, Dictionary<string, int> hamCountByWord)
        {
            var sumQ = 0.0;
            if (File.Exists(filePath))
            {
                var allLines = File.ReadAllLines(filePath);
                var words = allLines.Select(l => l.ToLowerInvariant().Split(' ')).SelectMany(w => w).ToList();

                foreach (var word in words)
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        var q = CalculateQ(word, spamFileCount, spamCountByWord, hamFileCount, hamCountByWord);
                        sumQ += q * ((double)spamFileCount / hamFileCount);
                    }
                }
                return (sumQ / words.Count);
            }
            else
            {
                throw new FileNotFoundException($"File '{filePath}' was not found.");
            }
        }

        //directly taken from:
        //https://netmatze.wordpress.com/2013/05/25/building-a-simple-spam-filter-with-naive-bayes-classifier-and-c/
        private double CalculateQ(string word,
            int countSpamMails, Dictionary<string, int> spamWordList,
            int countNotSpamMails, Dictionary<string, int> notSpamWordList)
        {
            if (spamWordList.ContainsKey(word) || notSpamWordList.ContainsKey(word))
            {
                double wordCountSpam = 1;
                if (spamWordList.ContainsKey(word))
                {
                    wordCountSpam = spamWordList[word];
                }

                double pWordIsSpam = 0.5 * wordCountSpam / countSpamMails;
                double wordCountNotSpam = 1;
                if (notSpamWordList.ContainsKey(word))
                {
                    wordCountNotSpam = notSpamWordList[word];
                }

                double pWordIsNotSpam = 0.5 * wordCountNotSpam / countNotSpamMails;
                double q = pWordIsSpam / pWordIsNotSpam;
                return q;
            }

            return 1;
        }
    }
}