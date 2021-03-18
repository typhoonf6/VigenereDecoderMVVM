using System.Collections.Generic;

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
        /// Default constructor
        /// </summary>
        public DecipherItemViewModel(string cipheredText, string fileName)
        {
            CipheredText = cipheredText;
            FileName = fileName;
        }
    }
}
