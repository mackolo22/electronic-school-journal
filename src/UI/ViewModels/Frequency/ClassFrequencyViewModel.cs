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
    public class ClassFrequencyViewModel : TeacherManagingClassesBaseViewModel
    {
        private bool _termSelected;
        private WrappedAttendance _selectedTerm;

        public ClassFrequencyViewModel(ITableStorageRepository repository) : base(repository) { }

        public override string SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                _selectedSubject = value;
                OnPropertyChanged(nameof(SelectedSubject));
            }
        }

        public List<WrappedAttendance> Terms { get; set; }
        public WrappedAttendance SelectedTerm
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
                    // TODO: zrobić to
                    var attendances = new List<WrappedAttendance>();
                    if (!String.IsNullOrWhiteSpace(student.SerializedAttendances))
                    {
                        attendances = JsonConvert.DeserializeObject<List<WrappedAttendance>>(student.SerializedAttendances);
                        foreach (var attendance in attendances)
                        {
                            attendance.
                        }
                    }

                    var wrappedStudent = new WrappedStudent
                    {
                        Id = student.Id,
                        Number = number,
                        FullName = student.FullName,
                        AllGrades = grades
                    };

                    number++;
                    wrappedStudents.Add(wrappedStudent);
                }

                _studentsFromAllClasses.Add(teacherClass, wrappedStudents);
            }
        }

        protected override void UpdateListOfStudentsFromSelectedClass()
        {
            //Students = _studentsFromAllClasses[_selectedClass];
            //foreach (var wrappedStudent in Students)
            //{
            //    if (wrappedStudent. != null && wrappedStudent.AllGrades.ContainsKey(_selectedSubject))
            //    {
            //        var grades = wrappedStudent.AllGrades[_selectedSubject];
            //        wrappedStudent.Grades = grades;
            //        if (wrappedStudent.Grades.Count == 0)
            //        {
            //            wrappedStudent.Average = String.Empty;
            //        }
            //        else
            //        {
            //            CountAverageForSelectedSubjectForGivenStudent(wrappedStudent);
            //        }
            //    }
            //    else
            //    {
            //        wrappedStudent.Grades = null;
            //        wrappedStudent.Average = String.Empty;
            //    }
            //}
        }
    }
}
