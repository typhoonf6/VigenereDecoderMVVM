using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        /// The frequency that letters typically show up in general english text
        /// </summary>
        private static readonly Dictionary<char, double> EnglishIC = new Dictionary<char, double>()
        {
            { 'A', .082 },
            { 'B', .015 },
            { 'C', .028 },
            { 'D', .043 },
            { 'E', .127 },
            { 'F', .022 },
            { 'G', .02 },
            { 'H', .061 },
            { 'I', .070 },
            { 'J', .002 },
            { 'K', .008 },
            { 'L', .04 },
            { 'M', .024 },
            { 'N', .067 },
            { 'O', .075 },
            { 'P', .019 },
            { 'Q', .001 },
            { 'R', .06 },
            { 'S', .063 },
            { 'T', .091 },
            { 'U', .028 },
            { 'V', .01 },
            { 'W', .023 },
            { 'X', .001 },
            { 'Y', .02 },
            { 'Z', .001 }
        };

        #region Dictionary
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
        #endregion

        #region Frequency Analysis
        /// <summary>
        /// Finds the key for a vigenere ciphered string using frequency analysis
        /// </summary>
        /// <param name="decipherItem"></param>
        /// <param name="maxKeyLength"></param>
        /// <returns></returns>
        public static int FindKeyUsingFrequency(DecipherItemViewModel decipherItem, int maxKeyLength, HashSet<string> dictionary, 
            int matchCount, int matchLength, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            GetCosetData(decipherItem, maxKeyLength);
            FillX2Dict(decipherItem);
            foreach (var ckvp in decipherItem.Cosets.Reverse().Take(3))
            {
                var sb = new StringBuilder();
                foreach (var x2List in ckvp.Value.Select(x => x.X2List.Values))
                {
                    sb.Append(x2List[0]);
                }
                var testKey = sb.ToString();
                var decipheredText = Decipher(decipherItem.CipheredText, testKey);
                if (TestDecryption(decipheredText, dictionary, matchCount, matchLength))
                {
                    decipherItem.DecipheredText = decipheredText;
                    decipherItem.CipherKey = testKey;
                    return 0;
                }
            }
            decipherItem.CipheredText = "Couldn't Find It :-(";
            decipherItem.CipherKey = "Try brute forcing";
            return 0;
        }

        /// <summary>
        /// Fill the <see cref="FreqDecipherItemViewModel"/> with data required to permute the key
        /// </summary>
        /// <param name="decipherItem"></param>
        /// <param name="maxKeyLength"></param>
        private static void GetCosetData(DecipherItemViewModel decipherItem, int maxKeyLength)
        {
            foreach (var i in Enumerable.Range(2, maxKeyLength))
            {
                var cosets = GetCosets(decipherItem.SanitisedCipheredString, i);
                decipherItem.Cosets.Add(GetAVGIoC(cosets), cosets);
            }
        }

        /// <summary>
        /// Divides the given string into cosets using the passed length
        /// </summary>
        /// <param name="encryptedText">The text to be decrypted</param>
        /// <param name="length">Distance between each character in the coset</param>
        /// <returns></returns>
        private static List<Coset> GetCosets(string encryptedText, int length)
        {
            var cosetList = new List<Coset>();
            for (var c1 = 0; c1 < length; c1++)
            {
                var sb = new StringBuilder();

                for (var c2 = c1; c2 < encryptedText.Length; c2 += length)
                {
                    sb.Append(encryptedText[c2]);
                }
                cosetList.Add(new Coset { CosetString = sb.ToString(), CosetDistance = length });
            }
            return cosetList;
        }

        /// <summary>
        /// Returns the average index of coincidence for the given list of cosets.
        /// </summary>
        /// <param name="cosetList"></param>
        /// <returns></returns>
        private static double GetAVGIoC(List<Coset> cosetList)
        {
            double totalIoC = 0;
            foreach (var coset in cosetList)
            {
                double currentIoC = 0;
                foreach (var letter in EnglishIC.Keys)
                {
                    var count = coset.CosetString.Length -
                        coset.CosetString.Replace(letter.ToString(), string.Empty).Length;
                    currentIoC += GetIoC(coset.CosetString.Length, count);
                }
                totalIoC += currentIoC;
            }
            return totalIoC / cosetList.Count;
        }

        /// <summary>
        /// Gets all the X2's for the given coset
        /// </summary>
        /// <param name="coset"></param>
        /// <returns></returns>
        private static void FillX2Dict(DecipherItemViewModel decipherItem)
        {
            foreach (var ckvp in decipherItem.Cosets.Reverse().Take(3))
            {
                foreach (var coset in ckvp.Value)
                {
                    var tempList = new SortedList<double, char>();
                    foreach (var c in EnglishIC.Keys)
                    {
                        var shiftedCoset = ShiftString(coset.CosetString, c);

                        var count = shiftedCoset.Length;
                        double x2 = 0;
                        foreach (var c1 in EnglishIC.Keys)
                        {
                            var letterCount = count -
                                shiftedCoset.Replace(c1.ToString(), string.Empty).Length;
                            var ioc = GetIoC(count, letterCount);
                            x2 += GetX2(ioc, EnglishIC[c1]);
                        }
                        while (tempList.ContainsKey(x2))
                            x2 += 0.00000000001;
                        tempList.Add(x2, c);
                    }
                    coset.X2List = tempList;
                }
            }
        }

        /// <summary>
        /// Mathematical function to find Index of Coincidence
        /// </summary>
        static Func<double, double, double> GetIoC = (x, y) => (1 / (x * (x - 1))) * (y * (y - 1));

        /// <summary>
        /// Mathematical function to find x2
        /// </summary>
        static Func<double, double, double> GetX2 = (f1, F1) => Math.Pow(f1 - F1, 2) / F1;
        #endregion

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

        /// <summary>
        /// Shifts string by a single character
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cipher"></param>
        /// <returns></returns>
        private static string ShiftString(string input, char cipher)
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
        #endregion
    }
}
