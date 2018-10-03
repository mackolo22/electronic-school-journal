using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using System.Windows;
using UI.Helpers;
using UI.ViewModels;

namespace UI.Dialogs
{
    public class AddPersonViewModel : ViewModelBase
    {
        protected readonly ILoginService _loginService;
        protected string _firstName;
        protected string _lastName;
        protected string _login;
        protected bool _loginGenerated;

        public AddPersonViewModel(ILoginService loginService)
        {
            _loginService = loginService;
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged(nameof(Login));
            }
        }

        public bool LoginGenerated
        {
            get => _loginGenerated;
            set
            {
                _loginGenerated = value;
                OnPropertyChanged(nameof(LoginGenerated));
            }
        }

        public string Password { get; set; }

        public bool ChangesSaved { get; set; }

        public RelayCommand GenerateLoginCommand => new RelayCommand(ExecuteGenerateLogin, () => true);
        protected void ExecuteGenerateLogin(object parameter)
        {
            try
            {
                Login = _loginService.GenerateLogin(FirstName, LastName);
                LoginGenerated = true;
            }
            catch (LoginException ex)
            {
                MessageBoxHelper.ShowErrorMessageBox("Błąd", ex.Message);
            }
        }

        public RelayCommand CancelCommand => new RelayCommand(ExecuteCancel, () => true);
        protected void ExecuteCancel(object parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }

        public RelayCommand SaveChangesCommand => new RelayCommand(ExecuteSaveChanges, () => true);
        protected void ExecuteSaveChanges(object parameter)
        {
            Password = _loginService.GeneratePassword();
            ChangesSaved = true;

            if (parameter is Window window)
            {
                window.Close();
            }
        }
    }
}
