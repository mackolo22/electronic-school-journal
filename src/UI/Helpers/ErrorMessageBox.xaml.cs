using System.Windows;

namespace UI.Helpers
{
    /// <summary>
    /// Interaction logic for ErrorMessageBox.xaml
    /// </summary>
    public partial class ErrorMessageBox : Window
    {
        public ErrorMessageBox(ErrorMessageBoxViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
