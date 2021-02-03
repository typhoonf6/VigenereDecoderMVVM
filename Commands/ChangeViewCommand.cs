using System;
using System.Windows.Input;

namespace VigenereDecoderMVVM
{
    /// <summary>
    /// Command that changes the UserControl displayed on the UI
    /// </summary>
    public class ChangeViewCommand : ICommand
    {
        /// <summary>
        /// An placeholder from the MainWindowViewModel
        /// </summary>
        readonly MainWindowViewModel windowViewModel;

        /// <summary>
        /// Event handler for the canexecute method
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Can be fired to force the <see cref="CanExecute(object)"/> method to be fired
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="viewModel">An instance of the <see cref="MainWindowViewModel"/> that called the command</param>
        public ChangeViewCommand(MainWindowViewModel viewModel)
        {
            windowViewModel = viewModel;
        }

        /// <summary>
        /// Evaluates the <see cref="MainWindowViewModel.IsRunning"/> bool to see if the command can be executed
        /// </summary>
        /// <param name="parameter">Parameter not used</param>
        /// <returns>This is bound to the IsEnabled button on the UI. If false, disables the button.</returns>
        public bool CanExecute(object parameter)
        {
            return windowViewModel.IsNotRunning;
        }

        /// <summary>
        /// Changes the UserControl to the selected value
        /// </summary>
        /// <param name="parameter">A string object passed from the UI to indicate which UserControl to switch to</param>
        public void Execute(object parameter)
        {
            if (parameter.ToString() == "DictionaryView")
                windowViewModel.SelectedViewModel = new DictionaryDecipherViewModel();
            if (parameter.ToString() == "BruteForceView")
                windowViewModel.SelectedViewModel = new BruteForceViewModel();
            if (parameter.ToString() == "FrequencyAnalysisView")
                windowViewModel.SelectedViewModel = new FrequencyAnalysisViewModel();
        }
    }
}
