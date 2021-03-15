using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace VigenereDecoderMVVM
{
    /// <summary>
    /// View model for the FrequencyAnalysis view
    /// </summary>
    class FrequencyAnalysisViewModel : UCBaseViewModel
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public FrequencyAnalysisViewModel()
        {
            BeginDecipherCommand = new RelayCommand(BeginDecipherAsync);
        }

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
                    foreach (var item in Items)
                    {
                        try
                        {
                            var _output = await Task.Run(() =>
                            DecipherProcessor.FindKeyUsingFrequency(item, 15, Dictionary, 6, 
                            5, CancellationTokenSource.Token));
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
    }
}

