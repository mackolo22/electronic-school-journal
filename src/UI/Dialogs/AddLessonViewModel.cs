using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System;
using System.Collections.ObjectModel;
using UI.ViewModels;

namespace UI.Dialogs
{
    public class AddLessonViewModel : ViewModelBase
    {
        private readonly IUniqueIDGenerator _uniqueIDGenerator;
        private Subject _subject;
        private Teacher _teacher;

        public AddLessonViewModel(IUniqueIDGenerator uniqueIDGenerator)
        {
            _uniqueIDGenerator = uniqueIDGenerator;

            Subjects = new ObservableCollection<Subject>
            {
                Subject.Maths,
                Subject.Polish
            };

            Teachers = new ObservableCollection<Teacher>();
        }

        // TODO: wygooglać jak zrobić listę ze wszystkich dostępnych pól w enumie.
        public ObservableCollection<Subject> Subjects { get; set; }

        public Subject Subject
        {
            get => _subject;
            set
            {
                _subject = value;
                OnPropertyChanged(nameof(Subject));
            }
        }

        public ObservableCollection<Teacher> Teachers { get; set; }

        public Teacher Teacher
        {
            get => _teacher;
            set
            {
                _teacher = value;
                OnPropertyChanged(nameof(Teacher));
            }
        }

        public RelayCommand AddTeacherCommand => new RelayCommand(ExecuteAddTeacher, () => true);
        private void ExecuteAddTeacher(object parameter)
        {
            var viewModel = UnityConfiguration.Resolve<AddTeacherViewModel>();
            var dialog = new AddTeacherDialog(viewModel);
            dialog.ShowDialog();

            if (viewModel.ChangesSaved)
            {
                long id = _uniqueIDGenerator.GetNextId();
                Teacher teacher = new Teacher(id)
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    User = new User
                    {
                        Login = viewModel.Login,
                        Password = viewModel.Password
                    }
                };

                Teachers.Add(teacher);
            }
        }

        public RelayCommand AddTermCommand => new RelayCommand(ExecuteAddTerm, () => true);
        private void ExecuteAddTerm(object parameter)
        {
            // TODO: dodać AddLessonTermDialog
        }
    }
}