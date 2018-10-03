using System.Windows;

namespace UI.Helpers
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class MessageBox : Window
    {
        public MessageBox(MessageBoxViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
