using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UI.ViewModels.WrappedModels;
using UI.Views;

namespace UI.ViewModels
{
    public class StudentGradesViewModel : BaseViewModel
    {
        private readonly IUsersRepository _usersRepository;

        public StudentGradesViewModel(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public long? StudentId { get; set; }

        public ObservableCollection<WrappedSubject> Subjects { get; set; }
        public string SelectedSubject { get; set; }

        public RelayCommand LoadedCommand => new RelayCommand(async (parameter) => await ExecuteLoadedAsync(parameter), () => true);
        private async Task ExecuteLoadedAsync(object parameter)
        {
            var user = await _usersRepository.GetAsync(nameof(Student), StudentId.ToString());
            Student student = user as Student;
            if (!String.IsNullOrWhiteSpace(student.SerializedGrades))
            {
                student.Grades = JsonConvert.DeserializeObject<Dictionary<string, List<Grade>>>(student.SerializedGrades);
                var subjects = new ObservableCollection<WrappedSubject>();
                foreach (var subjectName in student.Grades.Keys)
                {
                    var wrappedGrades = new ObservableCollection<WrappedGrade>();
                    var grades = student.Grades[subjectName];
                    if (grades.Count > 0)
                    {
                        double sum = 0;
                        foreach (var grade in grades)
                        {
                            wrappedGrades.Add(new WrappedGrade
                            {
                                Value = grade.Value,
                                Comment = grade.Comment,
                                LastModificationDate = grade.LastModificationDate
                            });

                            sum += grade.Value;
                        }

                        double average = sum / grades.Count;
                        var subject = new WrappedSubject
                        {
                            Name = subjectName,
                            Grades = wrappedGrades,
                            Average = String.Format("{0:0.00}", average)
                        };

                        subjects.Add(subject);
                    }
                }

                Subjects = subjects;
                OnPropertyChanged(nameof(Subjects));
            }
        }

        public RelayCommand ShowGradeCommand => new RelayCommand(ExecuteShowGrade, () => true);
        private void ExecuteShowGrade(object parameter)
        {
            var wrappedGrade = parameter as WrappedGrade;
            var viewModel = UnityConfiguration.Resolve<ShowGradeViewModel>();
            viewModel.Grade = wrappedGrade;
            var dialog = new ShowGradeDialog(viewModel);
            dialog.ShowDialog();
        }
    }
}
