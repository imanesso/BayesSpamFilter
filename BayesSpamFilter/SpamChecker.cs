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

        //As our normal Calculatioin did not work, we tried the following source:
        //https://netmatze.wordpress.com/2013/05/25/building-a-simple-spam-filter-with-naive-bayes-classifier-and-c/
        public bool IsSpam(string filePath, int spamFileCount, Dictionary<string, int> spamCountByWord, int hamFileCount, Dictionary<string, int> hamCountByWord)
        {
            var sumQ = 0.0;
            var wordCounter = 0;
            if (File.Exists(filePath))
            {
                var allLines = File.ReadAllLines(filePath);
                var words = allLines.Select(l => l.ToLowerInvariant().Split(' ')).SelectMany(w => w).Distinct().ToList();

                foreach (var word in words)
                {                    
                    var q = CalculateQ(word, spamFileCount, spamCountByWord, hamFileCount, hamCountByWord);
                    sumQ += q;
                    wordCounter++;
                }
                return sumQ / wordCounter > 1;
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
            double wordCountSpam = 1;
            if (spamWordList.ContainsKey(word))
            {
                wordCountSpam = spamWordList[word];
            }
            double Ph1e = 0.5 * wordCountSpam / countSpamMails;
            double wordCountNotSpam = 1;
            if (notSpamWordList.ContainsKey(word))
            {
                wordCountNotSpam = notSpamWordList[word];
            }
            double Ph2e = 0.5 * wordCountNotSpam / countNotSpamMails;
            double q = Ph1e / Ph2e;
            return q;
        }
    }
}