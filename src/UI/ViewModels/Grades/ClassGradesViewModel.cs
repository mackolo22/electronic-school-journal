using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UI.ViewModels.WrappedModels;
using UI.Views;

namespace UI.ViewModels
{
    public class ClassGradesViewModel : TeacherManagingClassesBaseViewModel
    {
        public ClassGradesViewModel(ITableStorageRepository repository) : base(repository) { }

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
                    Dictionary<string, ObservableCollection<WrappedGrade>> grades = null;
                    if (!String.IsNullOrWhiteSpace(student.SerializedGrades))
                    {
                        grades = JsonConvert.DeserializeObject<Dictionary<string, ObservableCollection<WrappedGrade>>>(student.SerializedGrades);
                        foreach (var listOfGrades in grades.Values)
                        {
                            foreach (var grade in listOfGrades)
                            {
                                grade.StudentId = student.Id;
                            }
                        }
                    }

                    var wrappedStudent = new WrappedStudent
                    {
                        Id = student.Id,
                        OrdinalNumber = number,
                        FullName = student.FullName,
                        AllGrades = grades
                    };

                    number++;
                    wrappedStudents.Add(wrappedStudent);
                }

                _studentsFromAllClasses.Add(teacherClass, wrappedStudents);
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
                    LessonSelected = false;
                }
                else
                {
                    UpdateListOfStudentsFromSelectedClass();
                    LessonSelected = true;
                }

                OnPropertyChanged(nameof(SelectedLesson));
                OnPropertyChanged(nameof(Students));
            }
        }

        protected override void UpdateListOfStudentsFromSelectedClass()
        {
            Students = _studentsFromAllClasses[_selectedClass];
            foreach (var wrappedStudent in Students)
            {
                if (wrappedStudent.AllGrades != null && wrappedStudent.AllGrades.ContainsKey(_selectedLesson.Subject))
                {
                    var grades = wrappedStudent.AllGrades[_selectedLesson.Subject];
                    wrappedStudent.Grades = grades;
                    if (wrappedStudent.Grades.Count == 0)
                    {
                        wrappedStudent.Average = String.Empty;
                    }
                    else
                    {
                        CountAverageForSelectedLessonForGivenStudent(wrappedStudent);
                    }
                }
                else
                {
                    wrappedStudent.Grades = null;
                    wrappedStudent.Average = String.Empty;
                }
            }
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
            addGradeViewModel.Title = "Wprowadzanie nowej oceny";
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
                        { SelectedLesson.Subject, wrappedStudent.Grades }
                    };
                }
                else
                {
                    if (wrappedStudent.AllGrades.ContainsKey(SelectedLesson.Subject))
                    {
                        wrappedStudent.AllGrades[SelectedLesson.Subject] = wrappedStudent.Grades;
                    }
                    else
                    {
                        wrappedStudent.AllGrades.Add(SelectedLesson.Subject, wrappedStudent.Grades);
                    }
                }

                CountAverageForSelectedLessonForGivenStudent(wrappedStudent);
                await UpdateGradesForGivenStudentAsync(wrappedStudent);
            }
        }

        public RelayCommand EditGradeCommand => new RelayCommand(async (parameter) => await ExecuteEditGradeAsync(parameter), () => true);
        private async Task ExecuteEditGradeAsync(object parameter)
        {
            var wrappedGrade = parameter as WrappedGrade;
            long? studentId = wrappedGrade.StudentId;
            var wrappedStudent = Students.Where(x => x.Id == studentId).FirstOrDefault();

            var addGradeViewModel = UnityConfiguration.Resolve<AddGradeViewModel>();
            addGradeViewModel.Title = "Edytowanie oceny";
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
                wrappedStudent.AllGrades[SelectedLesson.Subject] = wrappedStudent.Grades;
                CountAverageForSelectedLessonForGivenStudent(wrappedStudent);
                await UpdateGradesForGivenStudentAsync(wrappedStudent);
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
                CountAverageForSelectedLessonForGivenStudent(wrappedStudent);
            }

            await UpdateGradesForGivenStudentAsync(wrappedStudent);
        }

        private async Task UpdateGradesForGivenStudentAsync(WrappedStudent wrappedStudent)
        {
            string studentId = wrappedStudent.Id.ToString();
            var student = await _repository.GetAsync<Student>(nameof(Student), studentId);
            if (!String.IsNullOrWhiteSpace(student.SerializedGrades))
            {
                var givenStudentGrades = JsonConvert.DeserializeObject<Dictionary<string, List<Grade>>>(student.SerializedGrades);
                var grades = new List<Grade>();
                foreach (var wrappedGrade in wrappedStudent.Grades)
                {
                    grades.Add(new Grade
                    {
                        Value = wrappedGrade.Value,
                        Comment = wrappedGrade.Comment,
                        LastModificationDate = wrappedGrade.LastModificationDate
                    });
                }

                bool gradesContainSelectedLesson = givenStudentGrades.ContainsKey(SelectedLesson.Subject);
                if (gradesContainSelectedLesson)
                {
                    givenStudentGrades[SelectedLesson.Subject] = grades;
                    student.SerializedGrades = JsonConvert.SerializeObject(givenStudentGrades);
                }
                else
                {
                    givenStudentGrades.Add(SelectedLesson.Subject, grades);
                    student.SerializedGrades = JsonConvert.SerializeObject(givenStudentGrades);
                }
            }
            else
            {
                student.SerializedGrades = JsonConvert.SerializeObject(wrappedStudent.AllGrades);
            }

            await _repository.InsertOrReplaceAsync(student);
            OnPropertyChanged(nameof(Students));
        }

        private void CountAverageForSelectedLessonForGivenStudent(WrappedStudent wrappedStudent)
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
}
