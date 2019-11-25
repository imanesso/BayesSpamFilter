using System;
using System.Collections.Generic;

namespace BloomFilter
{
    class Program
    {
        public static string FileName = "words.txt";
        public static double ErrorProbability = 0.0001;

        static void Main(string[] args)
        {
            PrintHeader();

            // Load all words from the word.txt file
            FileReader fileReader = new FileReader(FileName);
            var words = fileReader.GetAllWords();

            // Create the word lists.
            var initialWords = new List<string>();
            var testWords = new List<string>();
            int counter = 1;
            foreach (var word in words)
            {
                // Every 4th word is for testing
                if (counter % 4 == 0)
                {
                    testWords.Add(word);
                }
                else
                {
                    initialWords.Add(word);
                }

                counter++;
            }

            BloomFilter bloomFilter = new BloomFilter(initialWords.Count, ErrorProbability);
            foreach (var initialWord in initialWords)
            {
                bloomFilter.AddWord(initialWord);
            }

            var errorCount = 0;
            foreach (var testWord in testWords)
            {
                // There are only unique words in the word list.
                // If the Bloom Filter tells us that the word is contained, this is an error.
                if (bloomFilter.ContainsWord(testWord))
                {
                    errorCount++;
                }
            }

            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Bloom-Filter finished");
            Console.WriteLine($"Used words to build the filter:     {initialWords.Count}");
            Console.WriteLine($"Used words to test the filter:      {testWords.Count}");
            Console.WriteLine($"Used error probability:             {ErrorProbability}");
            Console.WriteLine($"Calculated Bloom Filter size:       {bloomFilter.FilterSize}");
            Console.WriteLine($"Calculated number of has functions: {bloomFilter.NumberOfHashFunctions}");
            Console.WriteLine($"Expected errors:                    {Math.Round(testWords.Count * ErrorProbability, MidpointRounding.AwayFromZero)}");
            Console.WriteLine($"Actual errors:                      {errorCount}");
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();
            
            Console.WriteLine("Press any key to close the application");
            Console.ReadLine();
        }

        private static void PrintHeader()
        {
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Diskrete Stochastik");
            Console.WriteLine("Bloom-Filter");
            Console.WriteLine("Crated by Anessollah Ima & Jonathan Bättig");
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
