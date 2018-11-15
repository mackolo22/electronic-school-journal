using ApplicationCore.Extensions;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.ViewModels.WrappedModels;
using UI.Views;

namespace UI.ViewModels
{
    public class TimeTableViewModel : BaseViewModel
    {
        private readonly ITimeTableService _timeTableService;
        private readonly IUsersRepository _usersRepository;

        public TimeTableViewModel(ITimeTableService timeTableService, IUsersRepository usersRepository)
        {
            _timeTableService = timeTableService;
            _usersRepository = usersRepository;
        }

        public string UserType { get; set; }
        public User User { get; set; }
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
                    Lessons[i].Add(new WrappedLesson());
                }
            }

            List<Lesson> lessons = null;
            if (UserType == "Student")
            {
                var student = User as Student;
                lessons = await _timeTableService.GetLessonsForGivenClassAsync(student.ClassId);
            }
            else if (UserType == "Parent")
            {
                var parent = User as Parent;
                lessons = await _timeTableService.GetLessonsForGivenClassAsync(parent.ChildClassId);
            }
            else if (UserType == "Teacher")
            {
                var teacher = User as Teacher;
                lessons = await _timeTableService.GetLessonsForGivenTeacherAsync(teacher.Id);
            }

            CreateTimeTable(lessons);
            TimeTableLoaded = true;
            OnPropertyChanged(nameof(TimeTableLoaded));
        }

        private void CreateTimeTable(List<Lesson> lessons)
        {
            foreach (var lesson in lessons)
            {
                var wrappedLesson = new WrappedLesson
                {
                    Subject = lesson.Subject,
                    ClassName = lesson.ClassName,
                    Classroom = lesson.Classroom,
                    TeacherId = lesson.TeacherId,
                    Term = lesson.Term
                };

                Lessons[lesson.Term.LessonNumber][(int)lesson.Term.Day - 1] = wrappedLesson;
            }

            OnPropertyChanged(nameof(Lessons));
        }

        public RelayCommand ShowLessonCommand => new RelayCommand(async (parameter) => await ExecuteShowLessonAsync(parameter), () => true);
        private async Task ExecuteShowLessonAsync(object parameter)
        {
            if (parameter is WrappedLesson wrappedLesson)
            {
                var viewModel = new ShowLessonViewModel()
                {
                    Classroom = wrappedLesson.Classroom,
                    Subject = wrappedLesson.Subject.GetDisplayName()
                };

                if (UserType == "Student" || UserType == "Parent")
                {
                    var user = await _usersRepository.GetAsync(nameof(Teacher), wrappedLesson.TeacherId.ToString());
                    var teacher = user as Teacher;
                    viewModel.TeacherFullName = teacher.FullName;
                }
                else
                {
                    viewModel.ClassName = wrappedLesson.ClassName;
                }

                var dialog = new ShowLessonDialog(viewModel);
                dialog.ShowDialog();
            }
        }
    }
}
