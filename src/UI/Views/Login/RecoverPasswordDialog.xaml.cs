using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for RecoverPasswordDialog.xaml
    /// </summary>
    public partial class RecoverPasswordDialog : Window
    {
        public RecoverPasswordDialog(RecoverPasswordViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
