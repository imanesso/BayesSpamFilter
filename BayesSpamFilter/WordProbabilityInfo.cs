namespace BayesSpamFilter
{
    public class WordProbabilityInfo
    {
        public double HamProbability { get; }
        public double SpamProbability { get; }

        /// <summary>
        /// Constructor of The WordProbabilityInfo Class. This Object is used to store the probability of each word to be spam or ham.
        /// </summary>
        /// <param name="hamProbability"> Probability of a word to be ham. </param>
        /// <param name="spamProbability"> Probability of a word to be spam. </param>
        public WordProbabilityInfo(double hamProbability, double spamProbability)
        {
            HamProbability = hamProbability;
            SpamProbability = spamProbability;
        }
    }
}