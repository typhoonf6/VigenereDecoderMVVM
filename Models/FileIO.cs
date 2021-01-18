using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace VigenereDecoderMVVM
{
    // TODO Make this class async

    /// <summary>
    /// A helper class to retrieve the string data from a text file
    /// </summary>
    public static class FileIO
    {
        /// <summary>
        /// Full path to the word file
        /// </summary>
        private static string WordFilePath =
             System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "words.txt");

        /// <summary>
        /// Reads in the word dictionary from a text file
        /// </summary>
        /// <param name="path">Path to a text file full of english words</param>
        /// <returns>Set of english words from text file</returns>
        public static HashSet<string> GetWordSet()
        {
            var wordSet = new HashSet<string>();
            using (StreamReader sr = new StreamReader(WordFilePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    wordSet.Add(line.ToUpper());
                }
            }
            return wordSet;
        }

        /// <summary>
        /// Reads in text from an array of text file paths
        /// </summary>
        /// <param name="paths">Path to a text file containing an encoded string</param>
        /// <returns>An encoded string from a text file</returns>
        public static string GetDecipherString(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    return sr.ReadToEnd().ToUpper(); ;
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }
            return null;
        }
    }
}
