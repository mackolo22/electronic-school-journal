using ApplicationCore.Extensions;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace UI.ViewModels
{
    public class GradesViewModel : ViewModelBase
    {
        private readonly ITableStorageRepository _repository;
        private Dictionary<string, ObservableCollection<WrappedStudent>> _studentsFromAllClasses;
        private string _selectedClass;
        private bool _classSelected;
        private string _subject;

        public GradesViewModel(ITableStorageRepository repository)
        {
            _repository = repository;
        }

        public Teacher Teacher { get; internal set; }
        public List<string> TeacherClasses { get; set; }

        public List<string> Subjects { get; set; }

        public string Subject
        {
            get => _subject;
            set
            {
                _subject = value;
                OnPropertyChanged(nameof(Subject));

                foreach (var student in Students)
                {
                    if (student.AllGrades != null && student.AllGrades.ContainsKey(_subject))
                    {
                        var grades = student.AllGrades[_subject];
                        student.Grades = grades;
                        if (student.Grades.Count == 0)
                        {
                            student.Average = string.Empty;
                        }
                        else
                        {
                            double sum = 0;
                            foreach (var grade in student.Grades)
                            {
                                sum += grade;
                            }

                            double average = sum / student.Grades.Count;
                            student.Average = average.ToString();
                        }
                    }
                }

                OnPropertyChanged(nameof(Students));
            }
        }

        public bool ClassSelected
        {
            get => _classSelected;
            set
            {
                _classSelected = value;
                OnPropertyChanged(nameof(ClassSelected));
                Students = _studentsFromAllClasses[_selectedClass];
            }
        }

        public string SelectedClass
        {
            get => _selectedClass;
            set
            {
                _selectedClass = value;
                ClassSelected = true;
                OnPropertyChanged(nameof(SelectedClass));
            }
        }

        public ObservableCollection<WrappedStudent> Students { get; set; }

        public RelayCommand LoadedCommand => new RelayCommand(async (parameter) => await ExecuteLoadedAsync(parameter), () => true);
        private async Task ExecuteLoadedAsync(object parameter)
        {
            Teacher.Lessons = JsonConvert.DeserializeObject<List<Lesson>>(Teacher.SerializedLessons);
            var teacherClasses = new List<string>();
            Subjects = new List<string>();
            foreach (var lesson in Teacher.Lessons)
            {
                teacherClasses.Add(lesson.ClassName);
                Subjects.Add(lesson.Subject.GetDisplayName());
            }

            TeacherClasses = teacherClasses.Distinct().ToList();
            _studentsFromAllClasses = new Dictionary<string, ObservableCollection<WrappedStudent>>();
            foreach (var teacherClass in TeacherClasses)
            {
                var students = await _repository.GetAllByPropertyAsync<Student>(nameof(Student), "ClassId", teacherClass);
                var studentsAsList = students.OrderBy(x => x.FullName).ToList();
                var wrappedStudents = new ObservableCollection<WrappedStudent>();
                int number = 1;
                foreach (var student in studentsAsList)
                {
                    Dictionary<string, List<double>> grades = null;
                    if (!string.IsNullOrWhiteSpace(student.SerializedGrades))
                    {
                        grades = JsonConvert.DeserializeObject<Dictionary<string, List<double>>>(student.SerializedGrades);
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

            OnPropertyChanged(nameof(Subjects));
            OnPropertyChanged(nameof(TeacherClasses));
        }

        public RelayCommand AddGradeCommand => new RelayCommand(async (parameter) => await ExecuteAddGradeAsync(parameter), () => true);
        private async Task ExecuteAddGradeAsync(object parameter)
        {
            var wrappedStudent = parameter as WrappedStudent;
            if (wrappedStudent.Grades == null)
            {
                wrappedStudent.Grades = new List<double>();
            }

            wrappedStudent.Grades.Add(5);
            if (wrappedStudent.AllGrades == null)
            {
                wrappedStudent.AllGrades = new Dictionary<string, List<double>>
                {
                    { Subject, wrappedStudent.Grades }
                };
            }
            else
            {
                if (wrappedStudent.AllGrades.ContainsKey(Subject))
                {
                    wrappedStudent.AllGrades[Subject] = wrappedStudent.Grades;
                }
                else
                {
                    wrappedStudent.AllGrades.Add(Subject, wrappedStudent.Grades);
                }
            }

            double sum = 0;
            foreach (var grade in wrappedStudent.Grades)
            {
                wrappedStudent.AllGradesAsString += $"{grade} ";
                sum += grade;
            }

            double average = sum / wrappedStudent.Grades.Count;
            wrappedStudent.Average = average.ToString();
            var student = await _repository.GetAsync<Student>(nameof(Student), wrappedStudent.Id.ToString());
            student.SerializedGrades = JsonConvert.SerializeObject(wrappedStudent.AllGrades);
            await _repository.InsertOrReplaceAsync(student);
            OnPropertyChanged(nameof(Students));
        }
    }

    public class WrappedStudent
    {
        public long? Id { get; set; }
        public int Number { get; set; }
        public string FullName { get; set; }
        public List<double> Grades { get; set; }
        public IDictionary<string, List<double>> AllGrades { get; set; }
        public string AllGradesAsString { get; set; }
        public string Average { get; set; }
    }
}
