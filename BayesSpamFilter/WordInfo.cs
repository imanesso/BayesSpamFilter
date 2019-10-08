namespace BayesSpamFilter
{
    public class WordInfo
    {
        public decimal HamProbability { get; }
        public decimal SpamProbability { get; }

        public WordInfo(decimal hamProbability, decimal spamProbability)
        {
            HamProbability = hamProbability;
            SpamProbability = spamProbability;
        }
    }
}