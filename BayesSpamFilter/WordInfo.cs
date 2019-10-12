namespace BayesSpamFilter
{
    public class WordInfo
    {
        public double HamProbability { get; }
        public double SpamProbability { get; }

        public WordInfo(double hamProbability, double spamProbability)
        {
            HamProbability = hamProbability;
            SpamProbability = spamProbability;
        }
    }
}