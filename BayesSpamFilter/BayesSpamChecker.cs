using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BayesSpamFilter
{
    public class SpamChecker
    {
        public Dictionary<string, WordProbabilityInfo> WordInfoDictionary { get; }

        /// <summary>
        /// Class that contains the Bayes logic.
        /// </summary>
        /// <param name="wordInfoDictionary"> Dictionary with the word as the key and a WordProbabilityInfo as the value. </param>
        public SpamChecker(Dictionary<string, WordProbabilityInfo> wordInfoDictionary)
        {
            WordInfoDictionary = wordInfoDictionary;
        }

        /// <summary>
        /// Calculates the spam probability of an email by using naive Bayes algorithm.
        /// </summary>
        /// <param name="filePath"> Path to the email message. </param>
        /// <returns> A double Value between one and zero. 1 Meaning 100 % spam, 0 meaning 0% spam probability. </returns>
        public double GetSpamProbability(string filePath)
        {
            if (File.Exists(filePath))
            {
                var probabilitySum = 0d;
                var allLines = File.ReadAllLines(filePath);
                var words = allLines.Select(l => l.ToLowerInvariant().Split(' ')).SelectMany(w => w).Distinct().ToList();
                var wordCount = 0; // The number of words in the email contained in the WordInfoDictionary.
                foreach (var word in words)
                {
                    if (!string.IsNullOrEmpty(word))
                    {
                        if (WordInfoDictionary.Keys.Contains(word))
                        {
                            // This calculation is not really Bayes anymore. We had lots of underflow errors because of multiplication with very small numbers.
                            // We tried it with https://en.wikipedia.org/wiki/Naive_Bayes_spam_filtering#Combining_individual_probabilities which did not work.
                            // We also had a look at https://en.wikipedia.org/wiki/Naive_Bayes_spam_filtering#Other_expression_of_the_formula_for_combining_individual_probabilities. Still no lock.
                            // The decision was to use the basic idea of Kolmogoroff.
                            wordCount++;
                            // Calculate the probability of a given Word to be spam.
                            var wordProbability = WordInfoDictionary[word].SpamProbability /
                                                  (WordInfoDictionary[word].SpamProbability + WordInfoDictionary[word].HamProbability);
                            // Adding the spam probability the word to the overall spam probability.
                            probabilitySum += wordProbability;
                        }
                    }
                }

                // Calculate the probability by dividing the sum of all spam probabilities by the number of words.
                return probabilitySum / wordCount;
            }
            throw new FileNotFoundException();
        }

        public static List<string> GetDistinctWordsOfFile(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException();

            var allLines = File.ReadAllLines(path);
            var words = allLines.Select(l => l.ToLowerInvariant().Split(' ')).SelectMany(w => w).Distinct().ToList();

            return words;

        }
    }
}