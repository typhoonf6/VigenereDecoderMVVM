using System.Collections.Generic;

namespace VigenereDecoderMVVM
{
    public class Coset
    {
        /// <summary>
        /// A string of characters extracted from a vigenere ciphered string
        /// </summary>
        public string CosetString { get; set; }

        /// <summary>
        /// Distance between each character used to create the coset
        /// </summary>
        public int CosetDistance { get; set; }

        /// <summary>
        /// Dictionary containing the x2 values for each 
        /// </summary>
        public SortedList<double, char> X2List { get; set; }

        /// <summary>
        /// Holds the values for the top X2 candidate characters
        /// </summary>
        public List<char> TopX2 { get; set; }
    }
}
