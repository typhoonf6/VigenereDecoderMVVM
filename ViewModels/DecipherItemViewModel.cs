using System.Collections.Generic;
using System.Windows.Input;

namespace VigenereDecoderMVVM
{
    /// <summary>
    /// View model for each decipher item
    /// </summary>
    public class DecipherItemViewModel : BaseViewModel
    {
        /// <summary>
        /// Text to be deciphered
        /// </summary>
        public string CipheredText { get; set; }

        /// <summary>
        /// The deciphered text
        /// </summary>
        public string DecipheredText { get; set; }

        /// <summary>
        /// They key used to decipher the text
        /// </summary>
        public string CipherKey { get; set; }

        /// <summary>
        /// Filename of the item
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The ciphered string with all the special characters and whitespace removed.
        /// </summary>
        public string SanitisedCipheredString { get; set; }

        /// <summary>
        /// Sorted Dictionary containing the cosets for a particular key length with IoC as
        /// the key.
        /// </summary>
        public SortedDictionary<double, List<Coset>> Cosets { get; set; }

        /// <summary>
        /// The top 3 estimated key lengths for the given Ciphered Text
        /// </summary>
        public List<int> EstimatedKeyLengths { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DecipherItemViewModel(string cipheredText, string fileName)
        {
            CipheredText = cipheredText;
            SanitisedCipheredString = DecipherProcessor.SanitiseString(cipheredText);
            FileName = fileName;
            Cosets = new SortedDictionary<double, List<Coset>>();
        }
    }
}
