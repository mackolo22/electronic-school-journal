using System.Windows.Controls;
using UI.ViewModels;

namespace UI.Dialogs
{
    public class LoginViewModel : ViewModelBase
    {
        public string Login { get; set; }

        public RelayCommand LoginCommand => new RelayCommand(ExecuteLogin, () => true);
        private void ExecuteLogin(object parameter)
        {
            if (parameter is PasswordBox passwordBox)
            {
                string password = passwordBox.Password;
            }
        }
    }
}
