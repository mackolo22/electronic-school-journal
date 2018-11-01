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
        public Person Person { get; set; }
        public Student Student { get; set; }
        public Teacher Teacher { get; set; }
        public Parent Parent { get; set; }
        public Administrator Administrator { get; set; }

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
                string loggedAs = Person == null ? "Nie zalogowano." : $"Zalogowany jako: {Person.FullName}";
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
                else if (viewName == "AddClass")
                {
                    var viewModel = UnityConfiguration.Resolve<AddClassViewModel>();
                    viewModel.Administrator = Administrator;
                    CurrentViewModel = viewModel;
                }
                else if (viewName == "TimeTable")
                {
                    var viewModel = UnityConfiguration.Resolve<TimeTableViewModel>();
                    viewModel.Student = Student;
                    viewModel.Teacher = Teacher;
                    viewModel.Parent = Parent;
                    viewModel.Person = Person;
                    CurrentViewModel = viewModel;
                }
                else if (viewName == "Grades")
                {
                    if ((LoggedAsStudent || LoggedAsParent))
                    {
                        var viewModel = UnityConfiguration.Resolve<StudentGradesViewModel>();
                        long? studentId = (Student != null) ? Student.Id : Parent?.ChildId;
                        viewModel.StudentId = studentId;
                        CurrentViewModel = viewModel;
                    }
                    else if (LoggedAsTeacher)
                    {
                        var viewModel = UnityConfiguration.Resolve<ClassGradesViewModel>();
                        viewModel.Teacher = Teacher;
                        CurrentViewModel = viewModel;
                    }
                }
                else if (viewName == "Frequency")
                {
                    if ((LoggedAsStudent || LoggedAsParent))
                    {
                        var viewModel = UnityConfiguration.Resolve<StudentFrequencyViewModel>();
                        long? studentId = (Student != null) ? Student.Id : Parent?.ChildId;
                        viewModel.StudentId = studentId;
                        CurrentViewModel = viewModel;
                    }
                    else if (LoggedAsTeacher)
                    {
                        var viewModel = UnityConfiguration.Resolve<ClassFrequencyViewModel>();
                        viewModel.Teacher = Teacher;
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
            Person = null;
            Administrator = null;
            Student = null;
            Teacher = null;
            Parent = null;
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
                Person = viewModel.Person;
                UserLoggedIn = true;

                UserType = viewModel.UserType;
                switch (UserType)
                {
                    case "Administrator":
                        Administrator = viewModel.Administrator;
                        LoggedAsAdministrator = true;
                        OnPropertyChanged(nameof(LoggedAsAdministrator));
                        break;

                    case "Student":
                        Student = viewModel.Student;
                        LoggedAsStudent = true;
                        OnPropertyChanged(nameof(LoggedAsStudent));
                        break;

                    case "Teacher":
                        Teacher = viewModel.Teacher;
                        LoggedAsTeacher = true;
                        OnPropertyChanged(nameof(LoggedAsTeacher));
                        break;

                    case "Parent":
                        Parent = viewModel.Parent;
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
