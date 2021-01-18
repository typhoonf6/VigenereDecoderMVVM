using System.Windows.Controls;

namespace VigenereDecoderMVVM
{
    /// <summary>
    /// Interaction logic for BruteForceView.xaml
    /// </summary>
    public partial class BruteForceView : UserControl
    {
        public BruteForceView()
        {
            InitializeComponent();

            DataContext = new BruteForceViewModel();
        }
    }
}
