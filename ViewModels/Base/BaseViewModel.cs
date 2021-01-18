using PropertyChanged;
using System.ComponentModel;
using System.Windows.Input;

namespace VigenereDecoderMVVM
{
    /// <summary>
    /// Adds the foddy weaver package. This automatically implements the PropertyChanged
    /// method on any Properties within the class
    /// </summary>
    [AddINotifyPropertyChangedInterface]

    /// <summary>
    /// Base view model that fires property event changes
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The event that is fired is when any child property changes its value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
    }
}