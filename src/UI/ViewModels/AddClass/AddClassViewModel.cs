using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UI.Views;

namespace UI.ViewModels
{
    public class AddClassViewModel : ViewModelBase
    {
        private readonly IUniqueIDGenerator _uniqueIDGenerator;
        private readonly IClassService _classService;
        private readonly IPersonService _personService;
        private int _classNumber = 1;
        private string _classLetter = "A";
        private Teacher _educator;

        public AddClassViewModel(
            IUniqueIDGenerator uniqueIDGenerator,
            IClassService classService,
            IPersonService personService)
        {
            _uniqueIDGenerator = uniqueIDGenerator;
            _classService = classService;
            _personService = personService;

            Students = new ObservableCollection<Student>();
            Teachers = new ObservableCollection<Teacher>();
            Lessons = new ObservableCollection<Lesson>();
        }

        // TODO: trzeba będzie pobrać z bazy wszystkie dodane klasy i wrzucić do tych list te które pozostały niestworzone.
        public List<int> AvailableClassNumbers => new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
        public List<string> AvailableClassLetters => new List<string> { "A", "B", "C", "D" };

        public int ClassNumber
        {
            get => _classNumber;
            set
            {
                _classNumber = value;
                OnPropertyChanged(nameof(ClassNumber));
            }
        }

        public string ClassLetter
        {
            get => _classLetter;
            set
            {
                _classLetter = value;
                OnPropertyChanged(nameof(ClassLetter));
            }
        }

        public ObservableCollection<Teacher> Teachers { get; set; }

        public Teacher Educator
        {
            get => _educator;
            set
            {
                _educator = value;
                OnPropertyChanged(nameof(Educator));
            }
        }

        public ObservableCollection<Student> Students { get; set; }

        public ObservableCollection<Lesson> Lessons { get; set; }

        public RelayCommand LoadedCommand => new RelayCommand(async (parameter) => await ExecuteLoadedAsync(parameter), () => true);
        private async Task ExecuteLoadedAsync(object parameter)
        {
            var teachers = await _personService.GetAllTeachersAsync();
            foreach (var teacher in teachers)
            {
                if (!String.IsNullOrEmpty(teacher.SerializedLessons))
                {
                    teacher.Lessons = JsonConvert.DeserializeObject<List<Lesson>>(teacher.SerializedLessons);
                }
                else
                {
                    teacher.Lessons = new List<Lesson>();
                }

                Teachers.Add(teacher);
            }
        }

        public RelayCommand AddTeacherCommand => new RelayCommand(ExecuteAddTeacher, () => true);
        private void ExecuteAddTeacher(object parameter)
        {
            var viewModel = UnityConfiguration.Resolve<AddTeacherViewModel>();
            var dialog = new AddTeacherDialog(viewModel);
            dialog.ShowDialog();

            Teacher teacher = viewModel.Teacher;
            Teachers.Add(teacher);
            Educator = teacher;
        }

        public RelayCommand AddStudentCommand => new RelayCommand(ExecuteAddStudent, () => true);
        private void ExecuteAddStudent(object parameter)
        {
            // TODO: opakować to w DialogRequest i jakoś refleksją zdobywać widoki dla podanych view modeli.
            // var request = new DialogRequest(viewModel);
            // request.ShowDialog();
            var viewModel = UnityConfiguration.Resolve<AddStudentViewModel>();
            var dialog = new AddStudentDialog(viewModel);
            dialog.ShowDialog();

            if (viewModel.ChangesSaved)
            {
                long studentId = _uniqueIDGenerator.GetNextId();
                Student student = new Student(studentId)
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    ParentId = viewModel.Parent?.Id,
                    Parent = viewModel.Parent,
                    Login = viewModel.Login,
                    Password = viewModel.Password,
                    HashedPassword = viewModel.HashedPassword
                };

                if (student.Parent != null)
                {
                    student.Parent.ChildId = student.Id;
                }

                Students.Add(student);
            }
        }

        public RelayCommand AddLessonCommand => new RelayCommand(ExecuteAddLesson, () => true);
        private void ExecuteAddLesson(object parameter)
        {
            var viewModel = UnityConfiguration.Resolve<AddLessonViewModel>();
            viewModel.Teachers = Teachers;
            var dialog = new AddLessonDialog(viewModel);
            dialog.ShowDialog();

            if (viewModel.ChangesSaved)
            {
                Lesson lesson = new Lesson
                {
                    Subject = viewModel.Subject,
                    Teacher = viewModel.Teacher,
                    TeacherId = viewModel.Teacher?.Id,
                    Classroom = viewModel.Classroom,
                    Terms = viewModel.Terms
                };

                lesson.Teacher.Lessons.Add(lesson);
                Lessons.Add(lesson);
            }
        }

        public RelayCommand SaveChangesCommand => new RelayCommand(ExecuteSaveChanges, () => true);
        private async void ExecuteSaveChanges(object parameter)
        {
            // TODO: sprawdzenie czy pola są podane
            bool success = await _classService.AddNewClassAsync(ClassNumber, ClassLetter, Educator, Students, Lessons);
        }
    }
}
