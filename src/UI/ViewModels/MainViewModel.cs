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
        public bool IsOfflineMode { get; private set; }
        public User User { get; set; }
        public string MenuBackground { get; set; }
        public string ContentBackground { get; set; }

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
                else if (viewName == "Teachers")
                {
                    var viewModel = UnityConfiguration.Resolve<TeachersViewModel>();
                    viewModel.Administrator = User as Administrator;
                    CurrentViewModel = viewModel;
                }
                else if (viewName == "CreateClass")
                {
                    var viewModel = UnityConfiguration.Resolve<CreateClassViewModel>();
                    viewModel.Administrator = User as Administrator;
                    CurrentViewModel = viewModel;
                }
                else if (viewName == "ManageTimeTables")
                {
                    CurrentViewModel = UnityConfiguration.Resolve<ManageTimeTablesViewModel>();
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
                    var viewModel = UnityConfiguration.Resolve<MessagesViewModel>();
                    viewModel.User = User;
                    CurrentViewModel = viewModel;
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
                IsOfflineMode = viewModel.IsOfflineMode;
                User = viewModel.User;
                UserLoggedIn = true;

                UserType = viewModel.UserType;
                User = viewModel.User;
                switch (UserType)
                {
                    case "Administrator":
                        LoggedAsAdministrator = true;
                        OnPropertyChanged(nameof(LoggedAsAdministrator));
                        MenuBackground = "#FF3F3F46";
                        ContentBackground = "#FFBFBFBF";
                        break;

                    case "Student":
                        LoggedAsStudent = true;
                        OnPropertyChanged(nameof(LoggedAsStudent));
                        MenuBackground = "#FF0F1735";
                        ContentBackground = "#FFB8D3DC";
                        break;

                    case "Teacher":
                        LoggedAsTeacher = true;
                        OnPropertyChanged(nameof(LoggedAsTeacher));
                        MenuBackground = "#FF3B2B10";
                        ContentBackground = "#FFDACA99";
                        break;

                    case "Parent":
                        LoggedAsParent = true;
                        OnPropertyChanged(nameof(LoggedAsParent));
                        MenuBackground = "#FF0E3D06";
                        ContentBackground = "#FF84A77F";
                        break;
                }

                CurrentViewModel = UnityConfiguration.Resolve<HomeViewModel>();
                OnPropertyChanged(nameof(MenuBackground));
                OnPropertyChanged(nameof(ContentBackground));
                OnPropertyChanged(nameof(LoggedAs));
                OnPropertyChanged(nameof(UserLoggedIn));
            }
        }
    }
}
