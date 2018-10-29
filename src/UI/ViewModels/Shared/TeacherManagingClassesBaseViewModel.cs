using ApplicationCore.Extensions;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UI.ViewModels.WrappedModels;

namespace UI.ViewModels
{
    public abstract class TeacherManagingClassesBaseViewModel : BaseViewModel
    {
        protected string _selectedClass;
        protected bool _classSelected;
        protected WrappedLesson _selectedLesson;
        protected bool _lessonSelected;
        protected Dictionary<string, ObservableCollection<WrappedStudent>> _studentsFromAllClasses;
        protected readonly ITableStorageRepository _repository;

        public TeacherManagingClassesBaseViewModel(ITableStorageRepository repository)
        {
            _repository = repository;
        }

        public Teacher Teacher { get; set; }
        public List<string> TeacherClasses { get; set; }
        public List<WrappedLesson> Lessons { get; set; }
        public ObservableCollection<WrappedStudent> Students { get; set; }

        public string SelectedClass
        {
            get => _selectedClass;
            set
            {
                _selectedClass = value;
                UpdateListOfSubjectsForGivenClass();
                OnPropertyChanged(nameof(SelectedClass));
                ClassSelected = true;
            }
        }

        private void UpdateListOfSubjectsForGivenClass()
        {
            Lessons = new List<WrappedLesson>();
            foreach (var lesson in Teacher.Lessons)
            {
                if (lesson.ClassName == _selectedClass)
                {
                    var wrappedLesson = new WrappedLesson
                    {
                        Subject = lesson.Subject.GetDisplayName(),
                        Terms = lesson.Terms
                    };

                    Lessons.Add(wrappedLesson);
                }
            }

            OnPropertyChanged(nameof(Lessons));
        }

        public bool ClassSelected
        {
            get => _classSelected;
            set
            {
                _classSelected = value;
                OnPropertyChanged(nameof(ClassSelected));
            }
        }

        public abstract WrappedLesson SelectedLesson { get; set; }

        protected abstract void UpdateListOfStudentsFromSelectedClass();

        public bool LessonSelected
        {
            get => _lessonSelected;
            set
            {
                _lessonSelected = value;
                OnPropertyChanged(nameof(LessonSelected));
            }
        }

        public RelayCommand LoadedCommand => new RelayCommand(async (parameter) => await ExecuteLoadedAsync(parameter), () => true);
        private async Task ExecuteLoadedAsync(object parameter)
        {
            GetClassesForGivenTeacher();
            await GetAllStudentsFromAllTeacherClassesAsync();
            OnPropertyChanged(nameof(TeacherClasses));
        }

        private void GetClassesForGivenTeacher()
        {
            var teacherClasses = new List<string>();
            if (!String.IsNullOrWhiteSpace(Teacher.SerializedLessons))
            {
                Teacher.Lessons = JsonConvert.DeserializeObject<List<Lesson>>(Teacher.SerializedLessons);
                foreach (var lesson in Teacher.Lessons)
                {
                    teacherClasses.Add(lesson.ClassName);
                }
            }

            TeacherClasses = teacherClasses.Distinct().ToList();
        }

        protected abstract Task GetAllStudentsFromAllTeacherClassesAsync();
    }
}
