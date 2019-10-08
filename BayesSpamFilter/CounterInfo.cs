namespace BayesSpamFilter
{
    public class CounterInfo
    {
        public decimal HamCount { get; }
        public decimal SpamCount { get; }

        public CounterInfo(decimal hamCount, decimal spamCount)
        {
            HamCount = hamCount;
            SpamCount = spamCount;
        }
    }
}