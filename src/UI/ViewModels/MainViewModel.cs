using ApplicationCore.Models;
using System;
using System.Windows;
using UI.Views;

namespace UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _viewModel;
        private bool _loggedAsParent;
        private bool _loggedAsStudent;
        private bool _loggedAsTeacher;
        private bool _loggedAsAdministrator;

        public MainViewModel()
        {
            ViewModel = UnityConfiguration.Resolve<HomeViewModel>();
        }

        public string UserType { get; set; }
        public Person Person { get; set; }
        public Student Student { get; set; }
        public Teacher Teacher { get; set; }
        public Parent Parent { get; set; }
        public Administrator Administrator { get; set; }

        public ViewModelBase ViewModel
        {
            get => _viewModel;
            set
            {
                _viewModel = value;
                OnPropertyChanged(nameof(ViewModel));
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
                OnPropertyChanged(nameof(LoggedAsStudentOrParentVisibility));
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
                OnPropertyChanged(nameof(LoggedAsStudentOrParentVisibility));
            }
        }

        public Visibility LoggedAsStudentOrParentVisibility
        {
            get
            {
                if (LoggedAsStudent || LoggedAsParent)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
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
                switch (viewName)
                {
                    case "HomeView":
                        if (!(ViewModel is HomeViewModel))
                        {
                            ViewModel = UnityConfiguration.Resolve<HomeViewModel>();
                        }
                        break;

                    case "AddClassView":
                        if (!(ViewModel is AddClassViewModel))
                        {
                            ViewModel = UnityConfiguration.Resolve<AddClassViewModel>();
                        }
                        break;

                    case "TimeTableView":
                        if (!(ViewModel is TimeTableViewModel))
                        {
                            var viewModel = UnityConfiguration.Resolve<TimeTableViewModel>();
                            viewModel.Student = Student;
                            viewModel.Teacher = Teacher;
                            viewModel.Parent = Parent;
                            viewModel.Person = Person;
                            ViewModel = viewModel;
                        }
                        break;

                    case "TeacherGradesView":
                        if (!(ViewModel is TeacherGradesViewModel))
                        {
                            var viewModel = UnityConfiguration.Resolve<TeacherGradesViewModel>();
                            viewModel.Teacher = Teacher;
                            ViewModel = viewModel;
                        }
                        break;

                    case "StudentGradesView":
                        if (!(ViewModel is StudentGradesViewModel))
                        {
                            var viewModel = UnityConfiguration.Resolve<StudentGradesViewModel>();
                            long? studentId = (Student != null) ? Student.Id : Parent?.ChildId;
                            viewModel.StudentId = studentId;
                            ViewModel = viewModel;
                        }
                        break;
                }
            }
        }

        public RelayCommand LogoutCommand => new RelayCommand(ExecuteLogout, () => true);
        private void ExecuteLogout(object parameter)
        {
            ClearDataAboutLoggedUser();
            ViewModel = UnityConfiguration.Resolve<HomeViewModel>();
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

                OnPropertyChanged(nameof(LoggedAs));
                OnPropertyChanged(nameof(UserLoggedIn));
            }
        }
    }
}
