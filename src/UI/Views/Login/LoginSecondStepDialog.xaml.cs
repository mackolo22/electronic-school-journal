using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for LoginSecondStepDialog.xaml
    /// </summary>
    public partial class LoginSecondStepDialog : Window
    {
        public LoginSecondStepDialog(LoginSecondStepViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
