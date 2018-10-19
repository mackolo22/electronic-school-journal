using ApplicationCore.Enums;
using ApplicationCore.Models;
using System;
using UI.Views;

namespace UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _viewModel;

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

        public bool LoggedAsAdministrator { get; set; }
        public bool LoggedAsStudent { get; set; }
        public bool LoggedAsTeacher { get; set; }
        public bool LoggedAsParent { get; set; }

        public RelayCommand LoadedCommand => new RelayCommand(ExecuteLoaded, () => true);
        private void ExecuteLoaded(object parameter)
        {
            var viewModel = UnityConfiguration.Resolve<LoginFirstStepViewModel>();
            var dialog = new LoginFirstStepDialog(viewModel);
            dialog.ShowDialog();

            UserType = viewModel.UserType;
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

            Student = viewModel.Student;
            Teacher = viewModel.Teacher;
            Parent = viewModel.Parent;
            Administrator = viewModel.Administrator;
            Person = viewModel.Person;
            OnPropertyChanged(nameof(LoggedAs));
        }

        public RelayCommand ChangeViewCommand
        {
            get => new RelayCommand(ExecuteChangeView, () => true);
        }

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
                            var timeTableViewModel = UnityConfiguration.Resolve<TimeTableViewModel>();
                            timeTableViewModel.Student = Student;
                            timeTableViewModel.Teacher = Teacher;
                            timeTableViewModel.Parent = Parent;
                            timeTableViewModel.Person = Person;
                            ViewModel = timeTableViewModel;
                        }
                        break;

                    case "TeacherGradesView":
                        if (!(ViewModel is TeacherGradesViewModel))
                        {
                            var gradesViewModel = UnityConfiguration.Resolve<TeacherGradesViewModel>();
                            gradesViewModel.Teacher = Teacher;
                            ViewModel = gradesViewModel;
                        }
                        break;
                }
            }
        }
    }
}
