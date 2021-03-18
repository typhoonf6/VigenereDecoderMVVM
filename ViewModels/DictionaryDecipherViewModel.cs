using System;
using System.Threading.Tasks;

namespace VigenereDecoderMVVM
{
    /// <summary>
    /// View model for the dictionary decipher view
    /// </summary>
    class DictionaryDecipherViewModel : UCBaseViewModel
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DictionaryDecipherViewModel()
        {
            BeginDecipherCommand = new RelayCommand(BeginDecipherAsync);
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
                            DictionaryDecipher.Begin(item, Dictionary, PassableWordCount,
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
