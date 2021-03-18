using System.Collections.Generic;
using System.Threading;

namespace VigenereDecoderMVVM
{
    class DictionaryDecipher
    {
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
        public static int Begin(DecipherItemViewModel decipherItem,
            HashSet<string> dictionary, int matchCount, int matchLength, CancellationToken cancellationToken)
        {
            decipherItem.DecipheredText = "Running...";
            foreach (string key in dictionary)
            {
                cancellationToken.ThrowIfCancellationRequested();
                decipherItem.CipherKey = "Testing: " + key;
                var decipheredText = DecipherHelper.Decipher(decipherItem.CipheredText, key);
                if (DecipherHelper.TestDecryption(decipheredText, dictionary, matchCount, matchLength))
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
    }
}
