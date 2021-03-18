using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VigenereDecoderMVVM
{
    class DecipherHelper
    {
        /// <summary>
        /// Tests the string contains english words
        /// </summary>
        /// <param name="decipheredText">The deciphered text to test</param>
        /// <param name="dictionary">Set of english words to test against</param>
        /// <param name="matchCount">Number of words to match. Lower numbers may lead to false positives</param>
        /// <param name="matchLength">Length of the word to be matched. Lower numbers may lead to false positives</param>
        /// <returns></returns>
        public static bool TestDecryption(string decipheredText, HashSet<string> dictionary, int matchCount, int matchLength)
        {
            var count = 0;
            List<string> matched = new List<string>();
            foreach (string word in decipheredText.Split(' '))
            {
                if (word.Length >= matchLength & dictionary.Contains(word))
                {
                    matched.Add(word);
                    count++;
                }
                if (count >= matchCount)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Uses the passed cipher to decipher the encrypted string
        /// </summary>
        /// <param name="encryptedString">The string to be deciphered</param>
        /// <param name="cipher">The key used in the decipher process</param>
        /// <returns>A shifted string using the vigenere method</returns>
        public static string Decipher(string encryptedString, string cipher)
        {
            var sb = new StringBuilder();
            var cipherIndex = 0;
            foreach (char c in encryptedString)
            {
                // If not an alpha just append to stringbuilder
                if (Char.IsLetter(c))
                {
                    sb.Append(CharLookup(c, cipher[cipherIndex]));
                    if (cipherIndex++ == cipher.Length - 1) cipherIndex = 0;
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Uses arithmetic to simulate a lookup on a vigenere square
        /// </summary>
        /// <param name="input">The char to be shifted</param>
        /// <param name="cipher">The reference char</param>
        /// <returns>A char that has been vigenere shifted according to the inputs</returns>
        private static char CharLookup(char input, char cipher)
        {
            var output = (input - cipher + 26) % 26;
            output += 'A';
            return (char)output;
        }

        /// <summary>
        /// Shifts string by a single character
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cipher"></param>
        /// <returns></returns>
        public static string ShiftString(string input, char cipher)
        {
            var sb = new StringBuilder();
            foreach (var c in input)
            {
                sb.Append(CharLookup(c, cipher));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns the input string with no special characters or spaces.
        /// </summary>
        /// <param name="encryptedString"></param>
        /// <returns></returns>
        public static string SanitiseString(string encryptedString)
        {
            return Regex.Replace(encryptedString, "[^a-zA-Z]", "").ToUpper();
        }
    }
}
