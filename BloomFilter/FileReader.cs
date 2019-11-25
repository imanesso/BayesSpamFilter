using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BloomFilter
{
    public class FileReader
    {
        private readonly string fileName;

        /// <summary>
        /// Constructor for the FileReader
        /// </summary>
        /// <param name="fileName"></param>
        public FileReader(string fileName)
        {
            this.fileName = fileName;
        }

        /// <summary>
        /// This function retrieves all words in the list and returns them.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllWords()
        {
            if (File.Exists(fileName))
            {
                return File.ReadAllLines(fileName).ToList();
            }

            throw new DirectoryNotFoundException();
        }
    }
}