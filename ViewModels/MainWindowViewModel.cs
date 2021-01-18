using System;
using System.Windows.Input;

namespace VigenereDecoderMVVM
{
    /// <summary>
    /// View model for the main window
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        private BaseViewModel selectedViewModel = new DictionaryDecipherViewModel();

        /// <summary>
        /// Object containing the views
        /// </summary>
        public BaseViewModel SelectedViewModel { get => selectedViewModel; set => selectedViewModel = value; }

        /// <summary>
        /// Command to change the view
        /// </summary>
        public ICommand ChangeViewCommand { get; set; }

        private bool isNotRunning = true;
        /// <summary>
        /// Keeps track of whether a decipher process is running
        /// </summary>
        public bool IsNotRunning { get { return isNotRunning; } set { isNotRunning = value; ChangeViewCommand.CanExecute(value); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindowViewModel()
        {
            ChangeViewCommand = new ChangeViewCommand(this);
        }
    }
}
