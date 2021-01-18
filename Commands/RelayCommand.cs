using System;
using System.Windows.Input;

namespace VigenereDecoderMVVM
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        /// <summary>
        /// Action to be performed
        /// </summary>
        private Action mAction;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="action">The action to be performed</param>
        public RelayCommand(Action action)
        {
            mAction = action;
        }

        /// <summary>
        /// Relay command that can always execute
        /// </summary>
        /// <param name="parameter">The action can always be performed</param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Runs the command
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            mAction();
        }
    }
}
