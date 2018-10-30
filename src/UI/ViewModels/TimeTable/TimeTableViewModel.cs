using ApplicationCore.Extensions;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Views;

namespace UI.ViewModels
{
    public class TimeTableViewModel : BaseViewModel
    {
        private readonly ITimeTableService _timeTableService;

        public TimeTableViewModel(ITimeTableService timeTableService)
        {
            _timeTableService = timeTableService;
        }

        public Person Person { get; set; }
        public Student Student { get; set; }
        public Teacher Teacher { get; set; }
        public Parent Parent { get; set; }
        public List<List<WrappedLesson>> Lessons { get; set; }
        public bool TimeTableLoaded { get; set; }

        public RelayCommand LoadedCommand => new RelayCommand(async (parameter) => await ExecuteLoadedAsync(parameter), () => true);
        private async Task ExecuteLoadedAsync(object parameter)
        {
            Lessons = new List<List<WrappedLesson>>();
            for (int i = 0; i < 8; i++)
            {
                Lessons.Add(new List<WrappedLesson>());

                for (int j = 0; j < 5; j++)
                {
                    Lessons[i].Add(null);
                }
            }

            if (Student != null)
            {
                await CreateTimeTableForStudentAsync(Student.ClassId);
            }
            else if (Parent != null)
            {
                await CreateTimeTableForStudentAsync(Parent.ChildClassId);
            }
            else if (Teacher != null)
            {
                await CreateTimeTableForTeacherAsync(Teacher.Id);
            }

            TimeTableLoaded = true;
            OnPropertyChanged(nameof(TimeTableLoaded));
        }

        private async Task CreateTimeTableForStudentAsync(string classId)
        {
            var lessons = await _timeTableService.GetLessonsForGivenClassAsync(classId);
            foreach (var lesson in lessons)
            {
                string subject = lesson.Subject.GetDisplayName();
                foreach (var term in lesson.Terms)
                {
                    var wrappedLesson = new WrappedLesson
                    {
                        Subject = subject,
                        Term = $"{term.Day.GetDisplayName()}, g. {term.Time}",
                        ClassName = lesson.ClassName,
                        Classroom = lesson.Classroom,
                        TeacherId = lesson.TeacherId
                    };

                    Lessons[term.LessonNumber][(int)term.Day - 1] = wrappedLesson;
                }
            }

            OnPropertyChanged(nameof(Lessons));
        }

        private async Task CreateTimeTableForTeacherAsync(long teacherId)
        {
            var lessons = await _timeTableService.GetLessonsForGivenTeacherAsync(teacherId);
            foreach (var lesson in lessons)
            {
                string subject = lesson.Subject.GetDisplayName();
                foreach (var term in lesson.Terms)
                {
                    var wrappedLesson = new WrappedLesson
                    {
                        Subject = subject,
                        Term = $"{term.Day.GetDisplayName()}, g. {term.Time}",
                        Classroom = lesson.Classroom,
                        ClassName = lesson.ClassName
                    };

                    Lessons[term.LessonNumber][(int)term.Day - 1] = wrappedLesson;
                }
            }

            OnPropertyChanged(nameof(Lessons));
        }

        public RelayCommand ShowLessonCommand => new RelayCommand(ExecuteShowLesson, () => true);
        private void ExecuteShowLesson(object parameter)
        {
            if (parameter == null)
            {
                return;
            }

            var wrappedLesson = parameter as WrappedLesson;

            var viewModel = UnityConfiguration.Resolve<ShowLessonViewModel>();
            viewModel.ClassName = wrappedLesson.ClassName;
            viewModel.Classroom = wrappedLesson.Classroom;
            viewModel.Subject = wrappedLesson.Subject;
            viewModel.TeacherId = wrappedLesson.TeacherId;
            viewModel.Term = wrappedLesson.Term;

            var dialog = new ShowLessonDialog(viewModel);
            dialog.ShowDialog();
        }

        public class WrappedLesson
        {
            public string Subject { get; set; }
            public string Term { get; set; }
            public long? TeacherId { get; set; }
            public string Classroom { get; set; }
            public string ClassName { get; set; }
        }
    }
}
