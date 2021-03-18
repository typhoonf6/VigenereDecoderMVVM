using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace VigenereDecoderMVVM
{
    class BruteForceDecipher
    {
        /// <summary>
        ///  Set of characters used in the decipher process
        /// </summary>
        private static readonly string Charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

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
        public static int Begin(DecipherItemViewModel decipherItem,
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
                    var result = DecipherHelper.Decipher(decipherItem.CipheredText, guess.ToString());
                    if (DecipherHelper.TestDecryption(result, dictionary, matchCount, matchLength))
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
    }
}
