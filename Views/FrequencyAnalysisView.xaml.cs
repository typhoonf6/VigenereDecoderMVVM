using System.Windows.Controls;

namespace VigenereDecoderMVVM
{
    /// <summary>
    /// Interaction logic for FrequencyAnalysisView.xaml
    /// </summary>
    public partial class FrequencyAnalysisView : UserControl
    {
        public FrequencyAnalysisView()
        {
            InitializeComponent();

            DataContext = new FrequencyAnalysisViewModel();
        }
    }
}
