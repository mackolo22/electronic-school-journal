using ApplicationCore.Enums;
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
        private bool _administratorChecked;
        private bool _studentChecked;
        private bool _teacherChecked;
        private bool _parentChecked;
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
        public UserType UserType { get; set; }
        public string Login { get; set; }
        public bool LoggedIn { get; set; }

        public bool AdministratorChecked
        {
            get => _administratorChecked;
            set
            {
                UserType = UserType.Administrator;
                _administratorChecked = value;
                _studentChecked = _teacherChecked = _parentChecked = false;
                OnPropertyChanged(nameof(StudentChecked));
                OnPropertyChanged(nameof(TeacherChecked));
                OnPropertyChanged(nameof(ParentChecked));
            }
        }

        public bool StudentChecked
        {
            get => _studentChecked;
            set
            {
                UserType = UserType.Student;
                _studentChecked = value;
                _administratorChecked = _teacherChecked = _parentChecked = false;
                OnPropertyChanged(nameof(AdministratorChecked));
                OnPropertyChanged(nameof(TeacherChecked));
                OnPropertyChanged(nameof(ParentChecked));
            }
        }

        public bool TeacherChecked
        {
            get => _teacherChecked;
            set
            {
                UserType = UserType.Teacher;
                _teacherChecked = value;
                _administratorChecked = _studentChecked = _parentChecked = false;
                OnPropertyChanged(nameof(AdministratorChecked));
                OnPropertyChanged(nameof(StudentChecked));
                OnPropertyChanged(nameof(ParentChecked));
            }
        }

        public bool ParentChecked
        {
            get => _parentChecked;
            set
            {
                UserType = UserType.Parent;
                _parentChecked = value;
                _administratorChecked = _studentChecked = _teacherChecked = false;
                OnPropertyChanged(nameof(AdministratorChecked));
                OnPropertyChanged(nameof(StudentChecked));
                OnPropertyChanged(nameof(TeacherChecked));
            }
        }

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

        public RelayCommand LoginCommand => new RelayCommand(async (parameter) => await ExecuteLoginAsync(parameter), () => true);
        private async Task ExecuteLoginAsync(object parameter)
        {
            if (UserType == UserType.None)
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
                    case UserType.Student:
                        Student = await _loginService.LoginStudentAsync(Login, hashedPassword);
                        Person = Student;
                        break;

                    case UserType.Teacher:
                        Teacher = await _loginService.LoginTeacherAsync(Login, hashedPassword);
                        Person = Teacher;
                        break;

                    case UserType.Parent:
                        Parent = await _loginService.LoginParentAsync(Login, hashedPassword);
                        Person = Parent;
                        break;

                    case UserType.Administrator:
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
