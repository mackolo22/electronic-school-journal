using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using System;
using System.Windows;
using UI.Helpers;

namespace UI.ViewModels
{
    public class AddUserViewModel : BaseViewModel
    {
        protected readonly ILoginService _loginService;
        protected string _firstName;
        protected string _lastName;
        protected string _login;
        protected bool _loginGenerated;

        public AddUserViewModel(ILoginService loginService)
        {
            _loginService = loginService;
        }

        public bool ChangesSaved { get; set; }

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

        public string Email { get; set; }
        public string Password { get; set; }
        public string HashedPassword { get; set; }

        public RelayCommand GenerateLoginCommand => new RelayCommand(ExecuteGenerateLogin, () => true);
        protected void ExecuteGenerateLogin(object parameter)
        {
            try
            {
                Login = _loginService.GenerateLogin(FirstName, LastName);
            }
            catch (LoginException ex)
            {
                MessageBoxHelper.ShowErrorMessageBox(ex.Message);
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
        protected virtual void ExecuteSaveChanges(object parameter)
        {
            if (String.IsNullOrWhiteSpace(FirstName) || String.IsNullOrWhiteSpace(LastName) || String.IsNullOrWhiteSpace(Login) || String.IsNullOrWhiteSpace(Email))
            {
                ChangesSaved = false;
                MessageBoxHelper.ShowMessageBox("Wypełnij wszystkie pola.");
                return;
            }

            ChangesSaved = true;
            if (parameter is Window window)
            {
                window.Close();
            }
        }
    }
}
