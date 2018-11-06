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
    public class ClassFrequencyViewModel : TeacherManagingClassesBaseViewModel
    {
        private bool _termSelected;
        private LessonTerm _selectedTerm;
        private DateTime? _selectedDate;

        public ClassFrequencyViewModel(IUsersRepository usersRepository) : base(usersRepository) { }

        public override string SelectedClass
        {
            get => _selectedClass;
            set
            {
                _selectedClass = value;
                SelectedDate = null;
                Students = null;
                OnPropertyChanged(nameof(Students));
                UpdateListOfSubjectsForGivenClass();
                OnPropertyChanged(nameof(SelectedClass));
                ClassSelected = true;
            }
        }

        public override WrappedLesson SelectedLesson
        {
            get => _selectedLesson;
            set
            {
                _selectedLesson = value;
                if (_selectedLesson == null)
                {
                    Students = null;
                    SelectedDate = null;
                    LessonSelected = false;
                }
                else
                {
                    SelectedDate = null;
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
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));

                if (value == null)
                {
                    Students = null;
                    OnPropertyChanged(nameof(Students));
                    return;
                }

                DayOfWeek dayOfWeek = value.Value.DayOfWeek;
                int dayValue = (int)dayOfWeek;
                var terms = SelectedLesson.Terms.Where(x => (int)x.Day == dayValue);
                if (terms.Count() == 0)
                {
                    SelectedTerm = null;
                    MessageBoxHelper.ShowErrorMessageBox("Wybrane zajęcia nie odbywają się w dany dzień.");
                }
                else if (terms.Count() == 1)
                {
                    SelectedTerm = terms.First();
                }
                else if (terms.Count() > 1)
                {
                    var viewModel = new SelectLessonTimeViewModel
                    {
                        Terms = terms,
                        SelectedTerm = terms.First()
                    };

                    var dialog = new SelectLessonTimeDialog(viewModel);
                    dialog.ShowDialog();
                    SelectedTerm = viewModel.SelectedTerm;
                }
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
                var users = await _usersRepository.GetAllByPropertyAsync(nameof(Student), "ClassId", teacherClass);
                var students = users.OrderBy(x => x.FullName).Cast<Student>().ToList();
                var wrappedStudents = new ObservableCollection<WrappedStudent>();
                int number = 1;
                foreach (var student in students)
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
            var wrappedStudents = _studentsFromAllClasses[_selectedClass];
            foreach (var wrappedStudent in wrappedStudents)
            {
                if (wrappedStudent.Attendances == null || wrappedStudent.Attendances.Count == 0)
                {
                    wrappedStudent.AttendanceTypeInSelectedDay = AttendanceType.None;
                }
                else
                {
                    var attendance = wrappedStudent.Attendances
                        .Where(x => x.Subject == SelectedLesson.Subject
                               && x.LessonTerm.Day == SelectedTerm.Day
                               && x.LessonTerm.Time == SelectedTerm.Time
                               && x.Date == SelectedDate).FirstOrDefault();

                    if (attendance != null)
                    {
                        wrappedStudent.AttendanceTypeInSelectedDay = attendance.Type;
                    }
                    else
                    {
                        wrappedStudent.AttendanceTypeInSelectedDay = AttendanceType.None;
                    }
                }
            }

            Students = wrappedStudents;
        }

        public RelayCommand ChangeAttendanceForGivenStudentCommand
            => new RelayCommand(async (parameter) => await ExecuteChangeAttendanceForGivenStudentAsync(parameter), () => true);

        private async Task ExecuteChangeAttendanceForGivenStudentAsync(object parameter)
        {
            var wrappedStudent = parameter as WrappedStudent;
            Attendance attendance;
            string studentId = wrappedStudent.Id.ToString();
            var user = await _usersRepository.GetAsync(nameof(Student), studentId);
            Student student = user as Student;
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
                    attendance.Type = wrappedStudent.AttendanceTypeInSelectedDay;

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
                        Type = wrappedStudent.AttendanceTypeInSelectedDay,
                        TeacherFullName = Teacher.FullName
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
                    Type = wrappedStudent.AttendanceTypeInSelectedDay,
                    TeacherFullName = Teacher.FullName
                };

                wrappedStudent.Attendances.Add(attendance);
            }

            student.SerializedAttendances = JsonConvert.SerializeObject(wrappedStudent.Attendances);
            await _usersRepository.InsertOrReplaceAsync(student);
            OnPropertyChanged(nameof(Students));
        }
    }
}
