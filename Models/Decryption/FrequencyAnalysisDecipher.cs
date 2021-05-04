using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace VigenereDecoderMVVM
{
    class FrequencyAnalysisDecipher
    {
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

        /// <summary>
        /// Mathematical function to find Index of Coincidence
        /// </summary>
        static Func<double, double, double> GetIoC = (x, y) => (1 / (x * (x - 1))) * (y * (y - 1));

        /// <summary>
        /// Mathematical function to find x2
        /// </summary>
        static Func<double, double, double> GetX2 = (f1, F1) => Math.Pow(f1 - F1, 2) / F1;

        /// <summary>
        /// Finds the key for a vigenere ciphered string using frequency analysis
        /// </summary>
        /// <param name="decipherItem"></param>
        /// <param name="maxKeyLength"></param>
        /// <returns></returns>
        public static int Begin(FrequencyDecipherItemViewModel decipherItem, int maxKeyLength, HashSet<string> dictionary,
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
                var decipheredText = DecipherHelper.Decipher(decipherItem.CipheredText, testKey);
                if (DecipherHelper.TestDecryption(decipheredText, dictionary, matchCount, matchLength))
                {
                    decipherItem.DecipheredText = decipheredText;
                    decipherItem.CipherKey = testKey;
                    return 0;
                }
            }
            decipherItem.DecipheredText = "Couldn't Find It :-(";
            decipherItem.CipherKey = "Try brute forcing";
            return 0;
        }

        /// <summary>
        /// Fill the <see cref="FreqDecipherItemViewModel"/> with data required to permute the key
        /// </summary>
        /// <param name="decipherItem"></param>
        /// <param name="maxKeyLength"></param>
        private static void GetCosetData(FrequencyDecipherItemViewModel decipherItem, int maxKeyLength)
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
        private static void FillX2Dict(FrequencyDecipherItemViewModel decipherItem)
        {
            foreach (var ckvp in decipherItem.Cosets.Reverse().Take(5))
            {
                foreach (var coset in ckvp.Value)
                {
                    var tempList = new SortedList<double, char>();
                    foreach (var c in EnglishIC.Keys)
                    {
                        var shiftedCoset = DecipherHelper.ShiftString(coset.CosetString, c);

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
                            // Increment x2 to avoid collisions in list
                            x2 += 0.00000000001;
                        tempList.Add(x2, c);
                    }
                    coset.X2List = tempList;
                }
            }
        }
    }
}
