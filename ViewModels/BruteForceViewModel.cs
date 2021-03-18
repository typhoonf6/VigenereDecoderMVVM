using System;
using System.Threading.Tasks;

namespace VigenereDecoderMVVM
{
    /// <summary>
    /// View model for the brute force view
    /// </summary>
    class BruteForceViewModel : UCBaseViewModel
    {
        /// <summary>
        /// The length at which the brute force will start
        /// </summary>
        public int StartLength { get; set; } = 2;

        /// <summary>
        /// The length at which the brute force attempts will end
        /// </summary>
        public int EndLength { get; set; } = 5;

        /// <summary>
        /// Default constructor
        /// </summary>
        public BruteForceViewModel()
        {
            this.BeginDecipherCommand = new RelayCommand(BeginDecipherAsync);
        }

        #region Decipher Method
        /// <summary>
        /// Runs decipher proces for each viewmodel in Items
        /// </summary>
        private async void BeginDecipherAsync()
        {
            if (IsRunning)
                CancellationTokenSource.Cancel();
            if (Dictionary == null)
                Dictionary = FileIO.GetWordSet();
            if (Items != null && !IsRunning)
            {
                IsRunning = true;
                using (CancellationTokenSource = new System.Threading.CancellationTokenSource())
                {
                    foreach (DecipherItemViewModel item in Items)
                    {
                        try
                        {
                            var _output = await Task.Run(() =>
                            BruteForceDecipher.Begin
                            (item, Dictionary, StartLength, EndLength, PassableWordCount,
                            MinimumWordLength, CancellationTokenSource.Token));
                        }
#pragma warning disable CS0168 // Variable is declared but never used
                        catch (OperationCanceledException e)
#pragma warning restore CS0168 // Variable is declared but never used
                        {
                            item.DecipheredText = "Operation Cancelled.";
                            break;
                        }
                    }
                }
                IsRunning = false;
            }
        }
        #endregion
    }
}
