using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for LoginFirstStepDialog.xaml
    /// </summary>
    public partial class LoginFirstStepDialog : Window
    {
        public LoginFirstStepDialog(LoginFirstStepViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
