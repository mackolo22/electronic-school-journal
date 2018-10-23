using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UI.Helpers;
using UI.Views;

namespace UI.ViewModels
{
    public class LoginSecondStepViewModel : ViewModelBase
    {
        private Window _window;
        private LoginFirstStepViewModel _rootViewModel;
        private readonly ILoginService _loginService;
        private Dictionary<string, string> _userTypesWithPolishNames = new Dictionary<string, string>
        {
            { "Administrator", "administrator" },
            { "Student", "uczeń" },
            { "Teacher", "nauczyciel" },
            { "Parent", "rodzic" }
        };

        public LoginSecondStepViewModel(ILoginService loginService)
        {
            _loginService = loginService;
        }

        public LoginFirstStepViewModel RootViewModel
        {
            get => _rootViewModel;
            set
            {
                _rootViewModel = value;
                string polishUserType = _userTypesWithPolishNames[value.UserType];
                Title = $"Logowanie jako {polishUserType}";
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Title { get; set; }
        public string Login { get; set; }

        public RelayCommand LoadedCommand => new RelayCommand(ExecuteLoaded, () => true);
        private void ExecuteLoaded(object parameter)
        {
            if (parameter is Window window)
            {
                _window = window;
            }
        }

        public RelayCommand LoginCommand => new RelayCommand(async (parameter) => await ExecuteLoginAsync(parameter), () => true);
        private async Task ExecuteLoginAsync(object parameter)
        {
            if (String.IsNullOrWhiteSpace(Login))
            {
                MessageBoxHelper.ShowErrorMessageBox("Podaj login.");
                return;
            }

            if (parameter is PasswordBox passwordBox)
            {
                if (String.IsNullOrWhiteSpace(passwordBox.Password))
                {
                    MessageBoxHelper.ShowErrorMessageBox("Podaj hasło.");
                    return;
                }

                var dialog = new OperationInProgressDialog();
                dialog.Show();
                string hashedPassword = _loginService.HashPassword(passwordBox.Password);
                switch (RootViewModel.UserType)
                {
                    case "Student":
                        var student = await _loginService.LoginStudentAsync(Login, hashedPassword);
                        RootViewModel.Student = student;
                        RootViewModel.Person = student;
                        break;

                    case "Teacher":
                        var teacher = await _loginService.LoginTeacherAsync(Login, hashedPassword);
                        RootViewModel.Teacher = teacher;
                        RootViewModel.Person = teacher;
                        break;

                    case "Parent":
                        var parent = await _loginService.LoginParentAsync(Login, hashedPassword);
                        RootViewModel.Parent = parent;
                        RootViewModel.Person = parent;
                        break;

                    case "Administrator":
                        var administrator = await _loginService.LoginAdministratorAsync(Login, hashedPassword);
                        RootViewModel.Administrator = administrator;
                        RootViewModel.Person = administrator;
                        break;
                }

                dialog.Close();
                bool loggedIn = (RootViewModel.Person != null);
                RootViewModel.LoggedIn = loggedIn;
                if (loggedIn)
                {
                    _window.Close();
                }
                else
                {
                    MessageBoxHelper.ShowErrorMessageBox("Nie udało się zalogować!");
                }
            }
        }
    }
}
