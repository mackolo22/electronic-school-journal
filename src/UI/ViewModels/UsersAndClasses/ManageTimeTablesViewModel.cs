using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UI.Helpers;
using UI.ViewModels.WrappedModels;
using UI.Views;

namespace UI.ViewModels
{
    public class ManageTimeTablesViewModel : BaseViewModel
    {
        private readonly ITimeTableService _timeTableService;
        private readonly IClassesRepository _classesRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly LongRunningOperationHelper _longRunningOperationHelper;
        private StudentsClass _selectedClass;

        private static readonly string[] Times = new string[] { "8:00 - 8:45", "8:55 - 9:40", "9:50 - 10:35", "10:50 - 11:35", "11:45 - 12:30", "12:40 - 13:25", "13:30 - 14:15", "14:20 - 15:05" };
        private static readonly Day[] Days = Enum.GetValues(typeof(Day)).Cast<Day>().ToArray();

        public ManageTimeTablesViewModel(
            ITimeTableService timeTableService,
            IClassesRepository classesRepository,
            IUsersRepository usersRepository,
            LongRunningOperationHelper longRunningOperationHelper)
        {
            _timeTableService = timeTableService;
            _classesRepository = classesRepository;
            _usersRepository = usersRepository;
            _longRunningOperationHelper = longRunningOperationHelper;
        }

        public bool ClassSelected { get; set; }
        public ObservableCollection<ObservableCollection<WrappedLesson>> Lessons { get; set; }
        public List<StudentsClass> AllClasses { get; set; }

        public StudentsClass SelectedClass
        {
            get => _selectedClass;
            set
            {
                _selectedClass = value;
                ClassSelected = true;
                CreateMatrixOfLessons();
                if (!String.IsNullOrWhiteSpace(_selectedClass.SerializedLessons))
                {
                    CreateTimeTable();
                }
                else
                {
                    _selectedClass.Lessons = new List<Lesson>();
                }

                OnPropertyChanged(nameof(SelectedClass));
                OnPropertyChanged(nameof(ClassSelected));
                OnPropertyChanged(nameof(Lessons));
            }
        }

        private void CreateMatrixOfLessons()
        {
            Lessons = new ObservableCollection<ObservableCollection<WrappedLesson>>();
            for (int i = 0; i < 8; i++)
            {
                Lessons.Add(new ObservableCollection<WrappedLesson>());
                for (int j = 0; j < 5; j++)
                {
                    Day day = Days[j];
                    var wrappedLesson = new WrappedLesson
                    {
                        Day = day,
                        LessonNumber = i,
                        Time = Times[i]
                    };

                    Lessons[i].Add(wrappedLesson);
                }
            }
        }

        private void CreateTimeTable()
        {
            var lessons = JsonConvert.DeserializeObject<List<Lesson>>(_selectedClass.SerializedLessons);
            _selectedClass.Lessons = lessons;
            foreach (var lesson in lessons)
            {
                var wrappedLesson = new WrappedLesson
                {
                    Subject = lesson.Subject,
                    ClassName = lesson.ClassName,
                    Classroom = lesson.Classroom,
                    TeacherId = lesson.TeacherId,
                    Day = lesson.Term.Day,
                    LessonNumber = lesson.Term.LessonNumber,
                    Time = lesson.Term.Time
                };

                Lessons[lesson.Term.LessonNumber][(int)lesson.Term.Day - 1] = wrappedLesson;
            }
        }

        public RelayCommand LoadedCommand => new RelayCommand(async (parameter) => await ExecuteLoadedAsync(parameter), () => true);
        private async Task ExecuteLoadedAsync(object parameter)
        {
            await _longRunningOperationHelper.ProceedLongRunningOperationAsync(async () =>
            {
                var allClasses = await _classesRepository.GetAllAsync();
                AllClasses = allClasses.ToList();
                OnPropertyChanged(nameof(AllClasses));
            });
        }

        public RelayCommand AddLessonCommand => new RelayCommand(async (parameter) => await ExecuteAddLessonAsync(parameter), () => true);
        private async Task ExecuteAddLessonAsync(object parameter)
        {
            WrappedLesson newLesson = parameter as WrappedLesson;
            List<Teacher> allTeachers = null;
            await _longRunningOperationHelper.ProceedLongRunningOperationAsync(async () =>
            {
                var users = await _usersRepository.GetAllAsync(nameof(Teacher));
                allTeachers = users as List<Teacher>;
            });

            Teacher teacher = allTeachers.Where(x => x.Id == newLesson.TeacherId).FirstOrDefault();
            var viewModel = new AddLessonViewModel
            {
                AllTeachers = allTeachers,
                SelectedSubject = newLesson.Subject,
                SelectedTeacher = teacher,
                Classroom = newLesson.Classroom
            };

            var dialog = new AddLessonDialog(viewModel);
            dialog.ShowDialog();
            if (viewModel.ChangesSaved)
            {
                await _longRunningOperationHelper.ProceedLongRunningOperationAsync(async () =>
                {
                    newLesson.ClassName = _selectedClass.FullName;
                    newLesson.Subject = viewModel.SelectedSubject;
                    newLesson.Teacher = viewModel.SelectedTeacher;
                    newLesson.TeacherId = viewModel.SelectedTeacher.Id;
                    newLesson.Classroom = viewModel.Classroom;

                    LessonTerm term = new LessonTerm
                    {
                        Day = newLesson.Day,
                        Time = newLesson.Time,
                        LessonNumber = newLesson.LessonNumber
                    };

                    await UpdateLessonForClassAsync(newLesson, term);
                    await UpdateLessonForTeacherAsync(newLesson, term);
                });

                OnPropertyChanged(nameof(Lessons));
            }
        }

        private async Task UpdateLessonForClassAsync(WrappedLesson newLesson, LessonTerm term)
        {
            bool updatingLesson = false;
            foreach (var actualLesson in _selectedClass.Lessons)
            {
                if (actualLesson.Term.Day == term.Day && actualLesson.Term.LessonNumber == term.LessonNumber)
                {
                    // Doszło do zmiany nauczyciela, więc poprzedniemu należy usunąć tę lekcję.
                    if (actualLesson.TeacherId != newLesson.TeacherId)
                    {
                        await RemoveLessonFromPreviousTeacherAsync(newLesson, actualLesson, term);
                    }

                    actualLesson.ClassName = newLesson.ClassName;
                    actualLesson.Subject = newLesson.Subject;
                    actualLesson.TeacherId = newLesson.TeacherId;
                    actualLesson.Classroom = newLesson.Classroom;
                    updatingLesson = true;
                    break;
                }
            }

            if (!updatingLesson)
            {
                Lesson lesson = new Lesson
                {
                    ClassName = newLesson.ClassName,
                    Subject = newLesson.Subject,
                    TeacherId = newLesson.TeacherId,
                    Classroom = newLesson.Classroom,
                    Term = term
                };

                _selectedClass.Lessons.Add(lesson);
            }

            _selectedClass.SerializedLessons = JsonConvert.SerializeObject(_selectedClass.Lessons);
            await _classesRepository.InsertOrReplaceAsync(_selectedClass);
        }

        private async Task RemoveLessonFromPreviousTeacherAsync(WrappedLesson newLesson, Lesson actualLesson, LessonTerm term)
        {
            User user = await _usersRepository.GetAsync(nameof(Teacher), actualLesson.TeacherId.ToString());
            Teacher teacherToUpdate = user as Teacher;
            teacherToUpdate.Lessons = JsonConvert.DeserializeObject<List<Lesson>>(teacherToUpdate.SerializedLessons);
            Lesson lessonToRemove = teacherToUpdate.Lessons.Where(x => x.Term.Day == term.Day && x.Term.LessonNumber == term.LessonNumber).FirstOrDefault();
            int index = teacherToUpdate.Lessons.IndexOf(lessonToRemove);
            teacherToUpdate.Lessons.RemoveAt(index);
            teacherToUpdate.SerializedLessons = JsonConvert.SerializeObject(teacherToUpdate.Lessons);
            await _usersRepository.InsertOrReplaceAsync(teacherToUpdate);
        }

        private async Task UpdateLessonForTeacherAsync(WrappedLesson newLesson, LessonTerm term)
        {
            bool updatingLesson = false;
            if (!String.IsNullOrWhiteSpace(newLesson.Teacher.SerializedLessons))
            {
                newLesson.Teacher.Lessons = JsonConvert.DeserializeObject<List<Lesson>>(newLesson.Teacher.SerializedLessons);
            }
            else
            {
                newLesson.Teacher.Lessons = new List<Lesson>();
            }

            foreach (var actualLesson in newLesson.Teacher.Lessons)
            {
                if (actualLesson.Term.Day == term.Day && actualLesson.Term.LessonNumber == term.LessonNumber)
                {
                    actualLesson.ClassName = newLesson.ClassName;
                    actualLesson.Subject = newLesson.Subject;
                    actualLesson.TeacherId = newLesson.TeacherId;
                    actualLesson.Classroom = newLesson.Classroom;
                    updatingLesson = true;
                    break;
                }
            }

            if (!updatingLesson)
            {
                Lesson lesson = new Lesson
                {
                    ClassName = newLesson.ClassName,
                    Subject = newLesson.Subject,
                    TeacherId = newLesson.TeacherId,
                    Classroom = newLesson.Classroom,
                    Term = term
                };

                newLesson.Teacher.Lessons.Add(lesson);
            }

            newLesson.Teacher.SerializedLessons = JsonConvert.SerializeObject(newLesson.Teacher.Lessons);
            await _usersRepository.InsertOrReplaceAsync(newLesson.Teacher);
        }

        public class WrappedLesson : BindableObject
        {
            private Subject _subject;

            public Subject Subject
            {
                get => _subject;
                set
                {
                    _subject = value;
                    OnPropertyChanged(nameof(Subject));
                }
            }

            public long? TeacherId { get; set; }
            public Teacher Teacher { get; set; }
            public string Classroom { get; set; }
            public string ClassName { get; set; }
            public Day Day { get; set; }
            public string Time { get; set; }
            public int LessonNumber { get; set; }
        }
    }
}
