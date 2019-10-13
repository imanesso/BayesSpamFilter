namespace BayesSpamFilter
{
    public class WordProbabilityInfo
    {
        public double HamProbability { get; }
        public double SpamProbability { get; }

        public WordProbabilityInfo(double hamProbability, double spamProbability)
        {
            HamProbability = hamProbability;
            SpamProbability = spamProbability;
        }
    }
}