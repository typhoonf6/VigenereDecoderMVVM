using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VigenereDecoderMVVM
{
    /// <summary>
    /// View model for the FrequencyAnalysis view
    /// </summary>
    class FrequencyAnalysisViewModel : UCBaseViewModel
    {
        /// <summary>
        /// The maximum key length to assess. Can be quite high (max tested is 15) without affecting
        /// the speed of decryption
        /// </summary>
        public int MaxKeyLength { get; set; } = 15;

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
                    foreach (FrequencyDecipherItemViewModel item in Items)
                    {
                        try
                        {
                            var _output = await Task.Run(() =>
                            FrequencyAnalysisDecipher.Begin(item, MaxKeyLength, Dictionary, 6, 
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

        /// <summary>
        /// Opens a file browser dialog to add the text files containing the
        /// text to be deciphered
        /// </summary>
        public override void FileBrowser()
        {
            var fd = new OpenFileDialog()
            {
                Filter = "Text files (*.txt)|*.txt",
                Title = "Open text file"
            };

            fd.Multiselect = true;

            if (fd.ShowDialog() == DialogResult.OK)
            {
                Paths = fd.FileNames;
                Items = new ObservableCollection<BaseViewModel>();
                foreach (var file in Paths)
                {
                    Items.Add(new FrequencyDecipherItemViewModel(FileIO.GetDecipherString(file), Path.GetFileName(file)));
                }
            }
        }
    }
}

