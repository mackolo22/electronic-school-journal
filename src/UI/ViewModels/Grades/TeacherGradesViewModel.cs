using ApplicationCore.Extensions;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UI.Views;

namespace UI.ViewModels
{
    public class TeacherGradesViewModel : ViewModelBase
    {
        private readonly ITableStorageRepository _repository;
        private Dictionary<string, ObservableCollection<WrappedStudent>> _studentsFromAllClasses;
        private string _selectedClass;
        private bool _classSelected;
        private string _selectedSubject;
        private bool _subjectSelected;

        public TeacherGradesViewModel(ITableStorageRepository repository)
        {
            _repository = repository;
        }

        public Teacher Teacher { get; internal set; }
        public List<string> TeacherClasses { get; set; }
        public List<string> Subjects { get; set; }
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
            Subjects = new List<string>();
            foreach (var lesson in Teacher.Lessons)
            {
                if (lesson.ClassName == _selectedClass)
                {
                    Subjects.Add(lesson.Subject.GetDisplayName());
                }
            }

            OnPropertyChanged(nameof(Subjects));
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

        public string SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                _selectedSubject = value;
                if (_selectedSubject == null)
                {
                    Students = null;
                    SubjectSelected = false;
                }
                else
                {
                    UpdateListOfStudentsFromSelectedClass();
                    SubjectSelected = true;
                }

                OnPropertyChanged(nameof(SelectedSubject));
                OnPropertyChanged(nameof(Students));
            }
        }

        private void UpdateListOfStudentsFromSelectedClass()
        {
            Students = _studentsFromAllClasses[_selectedClass];
            foreach (var wrappedStudent in Students)
            {
                if (wrappedStudent.AllGrades != null && wrappedStudent.AllGrades.ContainsKey(_selectedSubject))
                {
                    var grades = wrappedStudent.AllGrades[_selectedSubject];
                    wrappedStudent.Grades = grades;
                    if (wrappedStudent.Grades.Count == 0)
                    {
                        wrappedStudent.Average = String.Empty;
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
        }

        public bool SubjectSelected
        {
            get => _subjectSelected;
            set
            {
                _subjectSelected = value;
                OnPropertyChanged(nameof(SubjectSelected));
            }
        }

        public RelayCommand LoadedCommand => new RelayCommand(async (parameter) => await ExecuteLoadedAsync(parameter), () => true);
        private async Task ExecuteLoadedAsync(object parameter)
        {
            Teacher.Lessons = JsonConvert.DeserializeObject<List<Lesson>>(Teacher.SerializedLessons);
            var teacherClasses = new List<string>();
            foreach (var lesson in Teacher.Lessons)
            {
                teacherClasses.Add(lesson.ClassName);
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
                    if (!String.IsNullOrWhiteSpace(student.SerializedGrades))
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
            addGradeViewModel.Title = $"Wprowadzanie nowej oceny - {wrappedStudent.FullName}";
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
                        { SelectedSubject, wrappedStudent.Grades }
                    };
                }
                else
                {
                    if (wrappedStudent.AllGrades.ContainsKey(SelectedSubject))
                    {
                        wrappedStudent.AllGrades[SelectedSubject] = wrappedStudent.Grades;
                    }
                    else
                    {
                        wrappedStudent.AllGrades.Add(SelectedSubject, wrappedStudent.Grades);
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

        public RelayCommand EditGradeCommand => new RelayCommand(async (parameter) => await ExecuteEditGradeAsync(parameter), () => true);
        private async Task ExecuteEditGradeAsync(object parameter)
        {
            var wrappedGrade = parameter as WrappedGrade;
            long? studentId = wrappedGrade.StudentId;
            var wrappedStudent = Students.Where(x => x.Id == studentId).FirstOrDefault();

            var addGradeViewModel = UnityConfiguration.Resolve<AddGradeViewModel>();
            addGradeViewModel.Title = $"Edytowanie oceny - {wrappedStudent.FullName}";
            addGradeViewModel.Grade = wrappedGrade.Value;
            addGradeViewModel.Comment = wrappedGrade.Comment;
            addGradeViewModel.LastModificationDate = wrappedGrade.LastModificationDate;
            var dialog = new AddGradeDialog(addGradeViewModel);
            dialog.ShowDialog();

            if (addGradeViewModel.ChangesSaved)
            {
                wrappedGrade.Value = addGradeViewModel.Grade;
                wrappedGrade.Comment = addGradeViewModel.Comment;
                wrappedGrade.LastModificationDate = DateTime.Now;

                if (wrappedStudent.AllGrades == null)
                {
                    wrappedStudent.AllGrades = new Dictionary<string, ObservableCollection<WrappedGrade>>
                    {
                        { SelectedSubject, wrappedStudent.Grades }
                    };
                }
                else
                {
                    if (wrappedStudent.AllGrades.ContainsKey(SelectedSubject))
                    {
                        wrappedStudent.AllGrades[SelectedSubject] = wrappedStudent.Grades;
                    }
                    else
                    {
                        wrappedStudent.AllGrades.Add(SelectedSubject, wrappedStudent.Grades);
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

        public RelayCommand RemoveGradeCommand => new RelayCommand(async (parameter) => await ExecuteRemoveGradeAsync(parameter), () => true);

        private async Task ExecuteRemoveGradeAsync(object parameter)
        {
            var wrappedGrade = parameter as WrappedGrade;
            long? studentId = wrappedGrade.StudentId;
            var wrappedStudent = Students.Where(x => x.Id == studentId).FirstOrDefault();
            wrappedStudent.Grades.Remove(wrappedGrade);

            if (wrappedStudent.Grades.Count() == 0)
            {
                wrappedStudent.Average = String.Empty;
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

            var student = await _repository.GetAsync<Student>(nameof(Student), wrappedStudent.Id.ToString());
            student.SerializedGrades = JsonConvert.SerializeObject(wrappedStudent.AllGrades);
            await _repository.InsertOrReplaceAsync(student);
            OnPropertyChanged(nameof(Students));
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
