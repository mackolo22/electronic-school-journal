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
        protected readonly IUsersRepository _usersRepository;

        public TeacherManagingClassesBaseViewModel(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public Teacher Teacher { get; set; }
        public List<string> TeacherClasses { get; set; }
        public List<WrappedLesson> Lessons { get; set; }
        public ObservableCollection<WrappedStudent> Students { get; set; }
        public abstract string SelectedClass { get; set; }

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
