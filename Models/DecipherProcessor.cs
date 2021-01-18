using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace VigenereDecoderMVVM
{
    /// <summary>
    /// Does the actual decipher operations on the string
    /// </summary>
    public static class DecipherProcessor
    {
        /// <summary>
        ///  Set of characters used in the decipher process
        /// </summary>
        private static readonly string Charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Attempts to decipher string contained in the decipher item using a hashset
        /// of words as the keys
        /// </summary>
        /// <param name="decipherItem">An instance of an item to be deciphered</param>
        /// <param name="dictionary">Hashset containing english words</param>
        /// <param name="matchCount">Number of words to match. Lower numbers may lead to false outputs</param>
        /// <param name="matchLength">Length of the word to be matched. Lower numbers may lead to false outputs</param>
        /// <param name="cancellationToken">Allows the operation to be cancelled</param>
        /// <returns>The DecipherItemViewModel is edited during the operation, an int is returned for the async/await operation only</returns>
        public static int DictionaryDecipher(DecipherItemViewModel decipherItem, 
            HashSet<string> dictionary, int matchCount, int matchLength, CancellationToken cancellationToken)
        {
            decipherItem.DecipheredText = "Running...";
            foreach (string key in dictionary)
            {
                cancellationToken.ThrowIfCancellationRequested();
                decipherItem.CipherKey = "Testing: " + key;
                var decipheredText = Decipher(decipherItem.CipheredText, key);
                if (TestDecryption(decipheredText, dictionary, matchCount, matchLength))
                {
                    decipherItem.DecipheredText = decipheredText;
                    decipherItem.CipherKey = key;
                    return 0;
                }
            }
            decipherItem.DecipheredText = "It couldn't be found :-(";
            decipherItem.CipherKey = "Try brute forcing!";
            return 0;
        }

        #region Brute Force
        /// <summary>
        /// Uses recursion to test all keys from startlength to endlength
        /// Is an extrapolation of the answer found at <see cref="https://stackoverflow.com/questions/14524808/c-sharp-bruteforce-starting-from-a-specified-point"/>
        /// </summary>
        /// <param name="decipherItem">An instance of an item to be deciphered</param>
        /// <param name="dictionary">Hashset containing english words</param>
        /// <param name="startLength">Length at which the brute force will start</param>
        /// <param name="endLength">Length at which the brute force will end</param>
        /// <param name="matchCount">Number of words to match. Lower numbers may lead to false outputs</param>
        /// <param name="matchLength">Length of the word to be matched. Lower numbers may lead to false outputs</param>
        /// <param name="cancellationToken">Allows the operation to be cancelled</param>
        /// <returns>The DecipherItemViewModel is edited during the operation, an int is returned for the async/await operation only</returns>
        public static int BruteForceDecipher(DecipherItemViewModel decipherItem, 
            HashSet<string> dictionary, int startLength, int endLength, 
            int matchCount, int matchLength, CancellationToken cancellationToken)
        {
            decipherItem.DecipheredText = "Running...";
            for (var number = startLength; number <= endLength; number++)
            {
                StringBuilder guess = new StringBuilder(number);
                for (var i = number; i > 0; i--)
                {
                    // I thought of making the start guess not all A's,
                    // but decided against it as this will account for 
                    // whether plain text is passed in.
                    guess.Append('A');
                }
                // Key will be null until something is found
                var result = Recurse(decipherItem, dictionary, 0, true, guess, 
                    matchCount, matchLength, cancellationToken);
                if (result != null)
                {
                    decipherItem.DecipheredText = result[0];
                    decipherItem.CipherKey = result[1];
                    return 0;
                }
            }
            decipherItem.DecipheredText = "It couldn't be found :-(";
            decipherItem.CipherKey = "Try increasing max length!";
            return 0;
        }

        /// <summary>
        /// Recursion method for the brute force operation.
        /// Is an extrapolation of the answer found at <see cref="https://stackoverflow.com/questions/14524808/c-sharp-bruteforce-starting-from-a-specified-point"/>
        /// </summary>
        /// <param name="decipherItem">An instance of an item to be deciphered</param>
        /// <param name="dictionary">Hashset containing english words</param>
        /// <param name="pos">Index of the guess that the recursion method is working on</param>
        /// <param name="firstPass">Indicated whether this is the first pass for the particular pos</param>
        /// <param name="guess">The current guess</param>
        /// <param name="matchCount">Number of words to match. Lower numbers may lead to false outputs</param>
        /// <param name="matchLength">Length of the word to be matched. Lower numbers may lead to false outputs</param>
        /// <param name="cancellationToken">Allows the operation to be cancelled</param>
        /// <returns>Returns a string array. Is null if it hasn't matched yet, else it contains the deciphered text and key</returns>
        private static string[] Recurse(DecipherItemViewModel decipherItem, 
            HashSet<string> dictionary, int pos, bool firstPass, StringBuilder guess, 
            int matchCount, int matchLength, CancellationToken cancellationToken)
        {
            for (int i = firstPass ? Charset.IndexOf(guess[pos]) : 0; i < Charset.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                guess[pos] = Charset[i];
                if (pos == guess.Length - 1)
                {
                    decipherItem.CipherKey = "Testing: " + guess.ToString();
                    var result = Decipher(decipherItem.CipheredText, guess.ToString());
                    if (TestDecryption(result, dictionary, matchCount, matchLength))
                        return new string[2] { result, guess.ToString() };
                }
                else
                {
                    var nextGuess = Recurse(decipherItem, dictionary, pos + 1, 
                        firstPass, guess, matchCount, matchLength, cancellationToken);
                    if (nextGuess != null)
                        return nextGuess;
                    firstPass = false;
                }
            }
            return null;
        }
        #endregion

        #region Helpers

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
        #endregion
    }
}
