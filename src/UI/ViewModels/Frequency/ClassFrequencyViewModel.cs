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
    public class ClassFrequencyViewModel : TeacherManagingClassesBaseViewModel
    {
        private bool _firstDateCheck = true;
        private bool _termSelected;
        private LessonTerm _selectedTerm;
        private DateTime? _selectedDate;

        public ClassFrequencyViewModel(ITableStorageRepository repository) : base(repository) { }

        public override WrappedLesson SelectedLesson
        {
            get => _selectedLesson;
            set
            {
                _selectedLesson = value;
                if (_selectedLesson == null)
                {
                    Students = null;
                    LessonSelected = false;
                }
                else
                {
                    SelectedDate = DateTime.Now.Date;
                    LessonSelected = true;
                }

                OnPropertyChanged(nameof(SelectedLesson));
                OnPropertyChanged(nameof(Students));
            }
        }

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                DayOfWeek dayOfWeek = value.Value.DayOfWeek;
                int dayValue = (int)dayOfWeek;
                var terms = SelectedLesson.Terms.Where(x => (int)x.Day == dayValue);
                if (terms.Count() == 0)
                {
                    _selectedDate = DateTime.Now.Date;
                    SelectedTerm = null;
                    if (_firstDateCheck)
                    {
                        _firstDateCheck = false;
                    }
                    else
                    {
                        MessageBoxHelper.ShowErrorMessageBox($"Wybrane zajęcia nie odbywają się w dany dzień.", "Uwaga");
                    }
                }
                else if (terms.Count() == 1)
                {
                    _selectedDate = value;
                    SelectedTerm = terms.First();
                }
                else if (terms.Count() > 1)
                {
                    _selectedDate = value;

                    var viewModel = new SelectLessonTimeViewModel
                    {
                        Terms = terms,
                        SelectedTerm = terms.First()
                    };

                    var dialog = new SelectLessonTimeDialog(viewModel);
                    dialog.ShowDialog();
                    SelectedTerm = viewModel.SelectedTerm;
                }

                OnPropertyChanged(nameof(SelectedDate));
            }
        }

        public LessonTerm SelectedTerm
        {
            get => _selectedTerm;
            set
            {
                _selectedTerm = value;
                if (_selectedTerm == null)
                {
                    Students = null;
                    TermSelected = false;
                }
                else
                {
                    UpdateListOfStudentsFromSelectedClass();
                    TermSelected = true;
                }

                OnPropertyChanged(nameof(SelectedTerm));
                OnPropertyChanged(nameof(Students));
            }
        }

        public bool TermSelected
        {
            get => _termSelected;
            set
            {
                _termSelected = value;
                OnPropertyChanged(nameof(TermSelected));
            }
        }

        protected override async Task GetAllStudentsFromAllTeacherClassesAsync()
        {
            _studentsFromAllClasses = new Dictionary<string, ObservableCollection<WrappedStudent>>();
            foreach (var teacherClass in TeacherClasses)
            {
                var students = await _repository.GetAllByPropertyAsync<Student>(nameof(Student), "ClassId", teacherClass);
                var studentsAsList = students.OrderBy(x => x.FullName).ToList();
                var wrappedStudents = new ObservableCollection<WrappedStudent>();
                int number = 1;
                foreach (var student in studentsAsList)
                {
                    var attendances = new ObservableCollection<Attendance>();
                    if (!String.IsNullOrWhiteSpace(student.SerializedAttendances))
                    {
                        attendances = JsonConvert.DeserializeObject<ObservableCollection<Attendance>>(student.SerializedAttendances);
                    }

                    var wrappedStudent = new WrappedStudent
                    {
                        Id = student.Id,
                        OrdinalNumber = number,
                        FullName = student.FullName,
                        Attendances = attendances
                    };

                    number++;
                    wrappedStudents.Add(wrappedStudent);
                }

                _studentsFromAllClasses.Add(teacherClass, wrappedStudents);
            }
        }

        protected override void UpdateListOfStudentsFromSelectedClass()
        {
            var students = _studentsFromAllClasses[_selectedClass];
            foreach (var student in students)
            {
                if (student.Attendances == null || student.Attendances.Count == 0)
                {
                    student.PresenceInSelectedDay = false;
                }
                else
                {
                    var attendance = student.Attendances
                        .Where(x => x.Subject == SelectedLesson.Subject
                               && x.LessonTerm.Day == SelectedTerm.Day
                               && x.LessonTerm.Time == SelectedTerm.Time
                               && x.Date == SelectedDate).FirstOrDefault();

                    if (attendance != null)
                    {
                        student.PresenceInSelectedDay = attendance.Presence;
                    }
                    else
                    {
                        student.PresenceInSelectedDay = false;
                    }
                }
            }

            Students = students;
        }

        public RelayCommand ChangeAttendanceForGivenStudentCommand
            => new RelayCommand(async (parameter) => await ExecuteChangeAttendanceForGivenStudentAsync(parameter), () => true);

        private async Task ExecuteChangeAttendanceForGivenStudentAsync(object parameter)
        {
            var wrappedStudent = parameter as WrappedStudent;
            Attendance attendance;
            string studentId = wrappedStudent.Id.ToString();
            var student = await _repository.GetAsync<Student>(nameof(Student), studentId);
            if (!String.IsNullOrWhiteSpace(student.SerializedAttendances))
            {
                var givenStudentAttendances = JsonConvert.DeserializeObject<List<Attendance>>(student.SerializedAttendances);
                attendance = givenStudentAttendances
                    .Where(x => x.Subject == SelectedLesson.Subject
                           && x.LessonTerm.Day == SelectedTerm.Day
                           && x.LessonTerm.Time == SelectedTerm.Time
                           && x.Date == SelectedDate).FirstOrDefault();

                if (attendance != null)
                {
                    attendance.Presence = wrappedStudent.PresenceInSelectedDay;

                    var att = wrappedStudent.Attendances
                        .Where(x => x.Subject == SelectedLesson.Subject
                               && x.LessonTerm.Day == SelectedTerm.Day
                               && x.LessonTerm.Time == SelectedTerm.Time
                               && x.Date == SelectedDate).FirstOrDefault();

                    int index = wrappedStudent.Attendances.IndexOf(att);
                    wrappedStudent.Attendances[index] = attendance;
                }
                else
                {
                    attendance = new Attendance
                    {
                        Date = SelectedDate.Value,
                        LessonTerm = SelectedTerm,
                        Subject = SelectedLesson.Subject,
                        Presence = wrappedStudent.PresenceInSelectedDay
                    };

                    wrappedStudent.Attendances.Add(attendance);
                }
            }
            else
            {
                attendance = new Attendance
                {
                    Date = SelectedDate.Value,
                    LessonTerm = SelectedTerm,
                    Subject = SelectedLesson.Subject,
                    Presence = wrappedStudent.PresenceInSelectedDay
                };

                wrappedStudent.Attendances.Add(attendance);
            }

            student.SerializedAttendances = JsonConvert.SerializeObject(wrappedStudent.Attendances);
            await _repository.InsertOrReplaceAsync(student);
            OnPropertyChanged(nameof(Students));
        }
    }
}
