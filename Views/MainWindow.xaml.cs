using System.Windows;
using System.Windows.Input;

namespace VigenereDecoderMVVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Default constructor. Adds the data context.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();

            /*
            var test = "B ajhmi s lfedexv lxlx txvemlx xzx hxzxk sfx mecxl xgh esfz";
            var dict = FileIO.GetWordSet();

            var result = DecipherProcessor.BruteForceDecipher(new DecipherItemViewModel(test, "Test"), dict, 2, 4, 4, 4, null);
            */
        }

        /// <summary>
        /// Allows to click and drag the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        /// <summary>
        /// Exits the applciation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PowerButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
