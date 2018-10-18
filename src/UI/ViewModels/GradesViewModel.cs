using ApplicationCore.Extensions;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UI.Dialogs;

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

                foreach (var wrappedStudent in Students)
                {
                    if (wrappedStudent.AllGrades != null && wrappedStudent.AllGrades.ContainsKey(_subject))
                    {
                        var grades = wrappedStudent.AllGrades[_subject];
                        wrappedStudent.Grades = grades;
                        if (wrappedStudent.Grades.Count == 0)
                        {
                            wrappedStudent.Average = string.Empty;
                        }
                        else
                        {
                            double sum = 0;
                            foreach (var grade in wrappedStudent.Grades)
                            {
                                sum += grade.Value;
                            }

                            double average = sum / wrappedStudent.Grades.Count;
                            wrappedStudent.Average = String.Format("{0:0.00}", average);
                        }
                    }
                    else
                    {
                        wrappedStudent.Grades = null;
                        wrappedStudent.Average = String.Empty;
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
                    Dictionary<string, ObservableCollection<WrappedGrade>> grades = null;
                    if (!string.IsNullOrWhiteSpace(student.SerializedGrades))
                    {
                        grades = JsonConvert.DeserializeObject<Dictionary<string, ObservableCollection<WrappedGrade>>>(student.SerializedGrades);
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
                wrappedStudent.Grades = new ObservableCollection<WrappedGrade>();
            }

            var addGradeViewModel = UnityConfiguration.Resolve<AddGradeViewModel>();
            var dialog = new AddGradeDialog(addGradeViewModel);
            dialog.ShowDialog();

            if (addGradeViewModel.ChangesSaved)
            {
                int id = wrappedStudent.Grades.Count + 1;
                var newGrade = new WrappedGrade
                {
                    Id = id,
                    StudentId = wrappedStudent.Id,
                    Value = addGradeViewModel.Grade,
                    Comment = addGradeViewModel.Comment,
                    LastModificationDate = DateTime.Now
                };

                wrappedStudent.Grades.Add(newGrade);
                if (wrappedStudent.AllGrades == null)
                {
                    wrappedStudent.AllGrades = new Dictionary<string, ObservableCollection<WrappedGrade>>
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
                    sum += grade.Value;
                }

                double average = sum / wrappedStudent.Grades.Count;
                wrappedStudent.Average = String.Format("{0:0.00}", average);
                var student = await _repository.GetAsync<Student>(nameof(Student), wrappedStudent.Id.ToString());
                student.SerializedGrades = JsonConvert.SerializeObject(wrappedStudent.AllGrades);
                await _repository.InsertOrReplaceAsync(student);
                OnPropertyChanged(nameof(Students));
            }
        }

        public RelayCommand ShowGradeCommand => new RelayCommand(async (parameter) => await ExecuteShowGradeAsync(parameter), () => true);
        private async Task ExecuteShowGradeAsync(object parameter)
        {
            var wrappedGrade = parameter as WrappedGrade;
            var addGradeViewModel = UnityConfiguration.Resolve<AddGradeViewModel>();
            addGradeViewModel.Grade = wrappedGrade.Value;
            addGradeViewModel.Comment = wrappedGrade.Comment;
            var dialog = new AddGradeDialog(addGradeViewModel);
            dialog.ShowDialog();

            if (addGradeViewModel.ChangesSaved)
            {
                wrappedGrade.Value = addGradeViewModel.Grade;
                wrappedGrade.Comment = addGradeViewModel.Comment;
                wrappedGrade.LastModificationDate = DateTime.Now;

                long? studentId = wrappedGrade.StudentId;
                var wrappedStudent = Students.Where(x => x.Id == studentId).FirstOrDefault();

                if (wrappedStudent.AllGrades == null)
                {
                    wrappedStudent.AllGrades = new Dictionary<string, ObservableCollection<WrappedGrade>>
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
                    sum += grade.Value;
                }

                double average = sum / wrappedStudent.Grades.Count;
                wrappedStudent.Average = String.Format("{0:0.00}", average);
                var student = await _repository.GetAsync<Student>(nameof(Student), wrappedStudent.Id.ToString());
                student.SerializedGrades = JsonConvert.SerializeObject(wrappedStudent.AllGrades);
                await _repository.InsertOrReplaceAsync(student);
                OnPropertyChanged(nameof(Students));
            }
        }
    }

    public class WrappedStudent : BindableObject
    {
        private ObservableCollection<WrappedGrade> _grades;
        private string _average;

        public long? Id { get; set; }
        public int Number { get; set; }
        public string FullName { get; set; }
        public ObservableCollection<WrappedGrade> Grades
        {
            get => _grades;
            set
            {
                _grades = value;
                OnPropertyChanged(nameof(Grades));
            }
        }
        public IDictionary<string, ObservableCollection<WrappedGrade>> AllGrades { get; set; }
        public string Average
        {
            get => _average;
            set
            {
                _average = value;
                OnPropertyChanged(nameof(Average));
            }
        }
    }

    public class WrappedGrade : BindableObject
    {
        private double _value;
        private string _comment;

        public int Id { get; set; }
        public long? StudentId { get; set; }
        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }

        }
        public string Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                OnPropertyChanged(nameof(Comment));
            }
        }
        public DateTime LastModificationDate { get; set; }
    }
}
