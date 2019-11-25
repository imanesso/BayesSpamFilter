using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Murmur;

namespace BloomFilter
{
    public class BloomFilter
    {
        private readonly byte[] bloomFilter;
        private List<HashAlgorithm> hashFunctions = new List<HashAlgorithm>();
        public int FilterSize { get; }
        public int NumberOfHashFunctions { get; }

        /// <summary>
        /// The constructor calculates all needed values for the Bloom Filter.
        /// </summary>
        /// <param name="wordCount"></param>
        /// <param name="errorProbability"></param>
        public BloomFilter(int wordCount, double errorProbability)
        {
            // Bloom Filter Size and the NumberOfHashFunctions is calculated by the wikipedia article.
            // Source: https://en.wikipedia.org/wiki/Bloom_filter#Optimal_number_of_hash_functions
            FilterSize = (int)Math.Ceiling(-1 * (wordCount * Math.Log(errorProbability) / Math.Pow(Math.Log(2), 2)));
            NumberOfHashFunctions = (int)Math.Ceiling(-1 * Math.Log(errorProbability, 2));
            bloomFilter = new byte[FilterSize];

            // Setup the list of HashFunctions with the calculated FilterSize.
            // Here, the Murmur library, which was mentioned in the task description, was used.
            for (var i = 0; i < NumberOfHashFunctions; i++)
            {
                hashFunctions.Add(MurmurHash.Create128((uint)i));
            }
        }

        /// <summary>
        /// Adds a specific word to the filter
        /// </summary>
        /// <param name="word"></param>
        public void AddWord(string word)
        {
            foreach (var hashAlgorithm in hashFunctions)
            {
                // Position is calculated by the hashCode of the word.
                // Modulo is used, since the hash code could be greater than the array length.
                var position = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(word)).GetHashCode() % bloomFilter.Length;
                bloomFilter[position] = 1;
            }
        }

        /// <summary>
        /// Checks if a word is already contained in the filter.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool ContainsWord(string word)
        {
            foreach (var hashAlgorithm in hashFunctions)
            {
                // Position is calculated by the hashCode of the word.
                // Modulo is used, since the hash code could be greater than the array length.
                var position = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(word)).GetHashCode() % bloomFilter.Length;

                // If the value at the calculated position is 0, this means the word is not yet contained.
                if (bloomFilter[position] == 0)
                {
                    return false;
                }

            }

            return true;
        }
    }
}