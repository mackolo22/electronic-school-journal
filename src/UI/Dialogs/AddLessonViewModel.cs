using ApplicationCore.Enums;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using UI.ViewModels;

namespace UI.Dialogs
{
    public class AddLessonViewModel : ViewModelBase
    {
        private Subject _subject;
        private Teacher _teacher;
        private string _classroom;

        public AddLessonViewModel()
        {
            Terms = new ObservableCollection<LessonTerm>();
        }

        public bool ChangesSaved { get; set; }

        public IEnumerable<Subject> Subjects
        {
            get
            {
                return Enum.GetValues(typeof(Subject)).Cast<Subject>();
            }
        }

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

        public string Classroom
        {
            get => _classroom;
            set
            {
                _classroom = value;
                OnPropertyChanged(nameof(Classroom));
            }
        }

        public ObservableCollection<LessonTerm> Terms { get; set; }

        public RelayCommand AddTeacherCommand => new RelayCommand(ExecuteAddTeacher, () => true);
        private void ExecuteAddTeacher(object parameter)
        {
            var viewModel = UnityConfiguration.Resolve<AddTeacherViewModel>();
            var dialog = new AddTeacherDialog(viewModel);
            dialog.ShowDialog();

            Teacher teacher = viewModel.Teacher;
            Teachers.Add(teacher);
            Teacher = teacher;
        }

        public RelayCommand AddTermCommand => new RelayCommand(ExecuteAddTerm, () => true);
        private void ExecuteAddTerm(object parameter)
        {
            var viewModel = UnityConfiguration.Resolve<AddTermViewModel>();
            var dialog = new AddTermDialog(viewModel);
            dialog.ShowDialog();

            string hour = viewModel.Hour;
            string minutes = viewModel.Minutes;
            string time = $"{hour}:{minutes}";
            LessonTerm term = new LessonTerm
            {
                Day = viewModel.Day,
                Time = time
            };

            Terms.Add(term);
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
            ChangesSaved = true;

            if (parameter is Window window)
            {
                window.Close();
            }
        }
    }
}