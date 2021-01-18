using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;

namespace VigenereDecoderMVVM
{
    /// <summary>
    /// BaseView model for UserControls
    /// </summary>
    class UCBaseViewModel : BaseViewModel
    {
        #region Attributes
        /// <summary>
        /// The command to begin the decipher process
        /// </summary>
        public ICommand BeginDecipherCommand { get; set; }

        /// <summary>
        /// Command to fire the filebrowser
        /// </summary>
        public ICommand FileBrowserCommand { get; set; }

        /// <summary>
        /// Once the text is deciphered, this is the amount of english word matches
        /// required to be detected as an english sentence
        /// </summary>
        public int PassableWordCount { get; set; } = 4;

        /// <summary>
        /// The minimum word length to check. i.e. checking on the word "a"
        /// will give too many false positives
        /// </summary>
        public int MinimumWordLength { get; set; } = 4;

        /// <summary>
        /// Just a collection for use in in the combobox on the UI
        /// </summary>
        public ObservableCollection<int> ComboBoxItems { get; } = new ObservableCollection<int> { 1, 2, 3, 4, 5 };

        /// <summary>
        /// Dictionary of english words
        /// </summary>
        public HashSet<string> Dictionary { get; set; }

        /// <summary>
        /// A list of paths to the files containing the ciphered strings
        /// </summary>
        //public string[] paths = { System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ciphertext.txt") };
        public string[] Paths { get; set; }

        /// <summary>
        /// List of all items to be displayed on the dictionary view model
        /// </summary>
        public ObservableCollection<DecipherItemViewModel> Items { get; set; }

        /// <summary>
        /// Text displayed on the button that begins the decipher process
        /// </summary>
        public string BeginButtonText { get; set; } = "Begin";

        /// <summary>
        /// Token thats passed into the decipher process to enable cancellation
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; set; }

        /// <summary>
        /// The private attribute for IsRunning property
        /// </summary>
        private bool _isRunning;
        /// <summary>
        /// Keeps track of whether decipher is running. Sets a few properties if true
        /// </summary>
        public bool IsRunning { 
            get
            {
                return _isRunning;
            } 
            set 
            {
                _isRunning = value;
                if (value)
                {
                    BeginButtonText = "Cancel";
                    UIInputsEnabled = false;
                }
                else
                {
                    BeginButtonText = "Begin";
                    UIInputsEnabled = true;
                }

                var DataContext = (MainWindowViewModel)App.Current.MainWindow.DataContext;
                DataContext.IsNotRunning = !value;
            }
        }

        /// <summary>
        /// User inputs get disabled if is IsRunning. This will essentially be the inverse of <see cref="IsRunning"/>.
        /// </summary>
        public bool UIInputsEnabled { get; set; } = true;

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public UCBaseViewModel()
        {
            this.FileBrowserCommand = new RelayCommand(FileBrowser);
        }

        #region Open File Dialog Method
        /// <summary>
        /// Opens a file browser dialog to add the text files containing the
        /// text to be deciphered
        /// </summary>
        public void FileBrowser()
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
                Items = new ObservableCollection<DecipherItemViewModel>();
                foreach (var file in Paths)
                {
                    Items.Add(new DecipherItemViewModel(FileIO.GetDecipherString(file), Path.GetFileName(file)));
                }
            }
        }
        #endregion
    }
}
