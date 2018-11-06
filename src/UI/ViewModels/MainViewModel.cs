using ApplicationCore.Models;
using System;
using UI.Views;

namespace UI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;
        private bool _loggedAsParent;
        private bool _loggedAsStudent;
        private bool _loggedAsTeacher;
        private bool _loggedAsAdministrator;

        public string UserType { get; set; }
        public User User { get; set; }

        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                if (_currentViewModel?.GetType() != value.GetType())
                {
                    _currentViewModel = value;
                    OnPropertyChanged(nameof(CurrentViewModel));
                }
            }
        }

        public string LoggedAs
        {
            get
            {
                string loggedAs = User == null ? "Nie zalogowano." : $"Zalogowany jako: {User.FullName}";
                return loggedAs;
            }
        }

        public bool UserLoggedIn { get; set; }

        public bool LoggedAsAdministrator
        {
            get => _loggedAsAdministrator;
            set
            {
                _loggedAsAdministrator = value;
                OnPropertyChanged(nameof(LoggedAsAdministrator));
            }
        }

        public bool LoggedAsStudent
        {
            get => _loggedAsStudent;
            set
            {
                _loggedAsStudent = value;
                OnPropertyChanged(nameof(LoggedAsStudent));
            }
        }

        public bool LoggedAsTeacher
        {
            get => _loggedAsTeacher;
            set
            {
                _loggedAsTeacher = value;
                OnPropertyChanged(nameof(LoggedAsTeacher));
            }
        }

        public bool LoggedAsParent
        {
            get => _loggedAsParent;
            set
            {
                _loggedAsParent = value;
                OnPropertyChanged(nameof(LoggedAsParent));
            }
        }

        public RelayCommand LoadedCommand => new RelayCommand(ExecuteLoaded, () => true);
        private void ExecuteLoaded(object parameter)
        {
            ProcessLoginOperation();
        }

        public RelayCommand ChangeViewCommand => new RelayCommand(ExecuteChangeView, () => true);
        private void ExecuteChangeView(object parameter)
        {
            string viewName = parameter as string;
            if (!String.IsNullOrWhiteSpace(viewName))
            {
                if (viewName == "Home")
                {
                    CurrentViewModel = UnityConfiguration.Resolve<HomeViewModel>();
                }
                else if (viewName == "Settings")
                {
                    var viewModel = UnityConfiguration.Resolve<SettingsViewModel>();
                    viewModel.UserType = UserType;
                    viewModel.User = User;
                    CurrentViewModel = viewModel;
                }
                else if (viewName == "AddClass")
                {
                    var viewModel = UnityConfiguration.Resolve<AddClassViewModel>();
                    viewModel.Administrator = User as Administrator;
                    CurrentViewModel = viewModel;
                }
                else if (viewName == "TimeTable")
                {
                    var viewModel = UnityConfiguration.Resolve<TimeTableViewModel>();
                    viewModel.UserType = UserType;
                    viewModel.User = User;
                    CurrentViewModel = viewModel;
                }
                else if (viewName == "Grades")
                {
                    if (LoggedAsStudent)
                    {
                        var viewModel = UnityConfiguration.Resolve<StudentGradesViewModel>();
                        long? studentId = User.Id;
                        viewModel.StudentId = studentId;
                        CurrentViewModel = viewModel;
                    }
                    else if (LoggedAsParent)
                    {
                        var viewModel = UnityConfiguration.Resolve<StudentGradesViewModel>();
                        Parent parent = User as Parent;
                        long? studentId = parent.ChildId;
                        viewModel.StudentId = studentId;
                        CurrentViewModel = viewModel;
                    }
                    else if (LoggedAsTeacher)
                    {
                        var viewModel = UnityConfiguration.Resolve<ClassGradesViewModel>();
                        viewModel.Teacher = User as Teacher;
                        CurrentViewModel = viewModel;
                    }
                }
                else if (viewName == "Frequency")
                {
                    if (LoggedAsStudent)
                    {
                        var viewModel = UnityConfiguration.Resolve<StudentFrequencyViewModel>();
                        long? studentId = User.Id;
                        viewModel.StudentId = studentId;
                        CurrentViewModel = viewModel;
                    }
                    else if (LoggedAsParent)
                    {
                        var viewModel = UnityConfiguration.Resolve<StudentFrequencyViewModel>();
                        Parent parent = User as Parent;
                        long? studentId = parent.ChildId;
                        viewModel.StudentId = studentId;
                        CurrentViewModel = viewModel;
                    }
                    else if (LoggedAsTeacher)
                    {
                        var viewModel = UnityConfiguration.Resolve<ClassFrequencyViewModel>();
                        viewModel.Teacher = User as Teacher;
                        CurrentViewModel = viewModel;
                    }
                }
                else if (viewName == "Messages")
                {
                    CurrentViewModel = UnityConfiguration.Resolve<CommunicationViewModel>();
                }
            }
        }

        public RelayCommand LogoutCommand => new RelayCommand(ExecuteLogout, () => true);
        private void ExecuteLogout(object parameter)
        {
            ClearDataAboutLoggedUser();
            CurrentViewModel = UnityConfiguration.Resolve<HomeViewModel>();
            ProcessLoginOperation();
        }

        private void ClearDataAboutLoggedUser()
        {
            UserLoggedIn = false;
            OnPropertyChanged(nameof(UserLoggedIn));
            User = null;
            UserType = String.Empty;
            LoggedAsAdministrator = LoggedAsStudent = LoggedAsTeacher = LoggedAsParent = false;
        }

        private void ProcessLoginOperation()
        {
            var viewModel = UnityConfiguration.Resolve<LoginFirstStepViewModel>();
            var dialog = new LoginFirstStepDialog(viewModel);
            dialog.ShowDialog();

            if (viewModel.LoggedIn)
            {
                User = viewModel.User;
                UserLoggedIn = true;

                UserType = viewModel.UserType;
                User = viewModel.User;
                switch (UserType)
                {
                    case "Administrator":
                        LoggedAsAdministrator = true;
                        OnPropertyChanged(nameof(LoggedAsAdministrator));
                        break;

                    case "Student":
                        LoggedAsStudent = true;
                        OnPropertyChanged(nameof(LoggedAsStudent));
                        break;

                    case "Teacher":
                        LoggedAsTeacher = true;
                        OnPropertyChanged(nameof(LoggedAsTeacher));
                        break;

                    case "Parent":
                        LoggedAsParent = true;
                        OnPropertyChanged(nameof(LoggedAsParent));
                        break;
                }

                CurrentViewModel = UnityConfiguration.Resolve<HomeViewModel>();
                OnPropertyChanged(nameof(LoggedAs));
                OnPropertyChanged(nameof(UserLoggedIn));
            }
        }
    }
}
