using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UI.Helpers;
using UI.ViewModels;

namespace UI.Dialogs
{
    public class LoginViewModel : ViewModelBase
    {
        private Window _window;
        private bool _closeApplicationOnWindowClosed = true;
        private readonly ILoginService _loginService;

        public LoginViewModel(ILoginService loginService)
        {
            _loginService = loginService;
        }

        public Person Person { get; set; }
        public Student Student { get; set; }
        public Teacher Teacher { get; set; }
        public Parent Parent { get; set; }
        public Administrator Administrator { get; set; }
        public string UserType { get; set; }
        public string Login { get; set; }
        public bool LoggedIn { get; set; }

        public RelayCommand LoadedCommand => new RelayCommand(ExecuteLoaded, () => true);
        private void ExecuteLoaded(object parameter)
        {
            if (parameter is Window window)
            {
                _window = window;
            }
        }

        public RelayCommand ClosedCommand => new RelayCommand(ExecuteClosed, () => true);
        private void ExecuteClosed(object parameter)
        {
            if (_closeApplicationOnWindowClosed)
            {
                App.ExitApplication();
            }
        }

        public RelayCommand SelectUserCommand => new RelayCommand(ExecuteSelectUser, () => true);
        private void ExecuteSelectUser(object parameter)
        {
            UserType = parameter as string;
        }

        public RelayCommand LoginCommand => new RelayCommand(async (parameter) => await ExecuteLoginAsync(parameter), () => true);
        private async Task ExecuteLoginAsync(object parameter)
        {
            if (String.IsNullOrWhiteSpace(UserType))
            {
                MessageBoxHelper.ShowErrorMessageBox("Wybierz typ użytkownika");
                return;
            }

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

                string hashedPassword = _loginService.HashPassword(passwordBox.Password);
                // TODO: zablokować jakoś UI na czas logowania
                switch (UserType)
                {
                    case "Student":
                        Student = await _loginService.LoginStudentAsync(Login, hashedPassword);
                        Person = Student;
                        break;

                    case "Teacher":
                        Teacher = await _loginService.LoginTeacherAsync(Login, hashedPassword);
                        Person = Teacher;
                        break;

                    case "Parent":
                        Parent = await _loginService.LoginParentAsync(Login, hashedPassword);
                        Person = Parent;
                        break;

                    case "Administrator":
                        Administrator = await _loginService.LoginAdministratorAsync(Login, hashedPassword);
                        Person = Administrator;
                        break;
                }

                LoggedIn = (Student != null) || (Teacher != null) || (Parent != null) || (Administrator != null);
                if (LoggedIn)
                {
                    _closeApplicationOnWindowClosed = false;
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
