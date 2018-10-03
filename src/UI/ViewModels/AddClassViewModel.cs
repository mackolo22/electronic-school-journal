using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UI.Dialogs;

namespace UI.ViewModels
{
    public class AddClassViewModel : ViewModelBase
    {
        private readonly IUniqueIDGenerator _uniqueIDGenerator;
        private readonly ITableStorageRepository _repository;
        private byte _classNumber = 1;
        private char _classLetter = 'A';
        private Teacher _educator;

        public AddClassViewModel(IUniqueIDGenerator uniqueIDGenerator, ITableStorageRepository repository)
        {
            _uniqueIDGenerator = uniqueIDGenerator;
            _repository = repository;

            Students = new ObservableCollection<Student>();
            Teachers = new ObservableCollection<Teacher>();
            Lessons = new ObservableCollection<Lesson>();
        }

        // TODO: trzeba będzie pobrać z bazy wszystkie dodane klasy i wrzucić do tych list te które pozostały niestworzone.
        public List<byte> AvailableClassNumbers => new List<byte> { 1, 2, 3, 4, 5, 6, 7, 8 };
        public List<char> AvailableClassLetters => new List<char> { 'A', 'B', 'C', 'D' };

        public byte ClassNumber
        {
            get => _classNumber;
            set
            {
                _classNumber = value;
                OnPropertyChanged(nameof(ClassNumber));
            }
        }

        public char ClassLetter
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

        public RelayCommand AddTeacherCommand => new RelayCommand(ExecuteAddTeacher, () => true);
        private void ExecuteAddTeacher(object parameter)
        {
            var viewModel = UnityConfiguration.Resolve<AddTeacherViewModel>();
            var dialog = new AddTeacherDialog(viewModel);
            dialog.ShowDialog();

            if (viewModel.ChangesSaved)
            {
                long id = _uniqueIDGenerator.GetNextId();
                Teacher teacher = new Teacher(id)
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    User = new User
                    {
                        Login = viewModel.Login,
                        Password = viewModel.Password
                    }
                };

                Teachers.Add(teacher);
            }
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
                long id = _uniqueIDGenerator.GetNextId();
                Student student = new Student(id)
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Parent = viewModel.Parent,
                    User = new User
                    {
                        Login = viewModel.Login,
                        Password = viewModel.Password
                    }
                };

                student.Parent.Child = student;
                Students.Add(student);
            }
        }

        public RelayCommand AddLessonCommand => new RelayCommand(ExecuteAddLesson, () => true);
        private void ExecuteAddLesson(object parameter)
        {
            var viewModel = UnityConfiguration.Resolve<AddLessonViewModel>();
            var dialog = new AddLessonDialog(viewModel);
            dialog.ShowDialog();
        }

        public RelayCommand SaveChangesCommand => new RelayCommand(ExecuteSaveChanges, () => true);
        private async void ExecuteSaveChanges(object parameter)
        {
            var studentsClass = new StudentsClass(ClassNumber, ClassLetter)
            {
                Educator = Educator,
                Students = Students
            };

            studentsClass.Educator.Class = studentsClass;
            foreach (var student in studentsClass.Students)
            {
                student.Class = studentsClass;
            }

            Dictionary<string, EntityProperty> flattenedProperties = EntityPropertyConverter.Flatten(studentsClass, null, null);
            var dynamicTableEntity = new DynamicTableEntity(studentsClass.PartitionKey, studentsClass.RowKey)
            {
                Properties = flattenedProperties
            };

            await _repository.InsertAsync(dynamicTableEntity);
            await _repository.InsertAsync(studentsClass.Educator);
            await _repository.InsertBatchAsync(studentsClass.Students);
            foreach (var student in studentsClass.Students)
            {
                if (student.Parent != null)
                {
                    await _repository.InsertAsync(student.Parent);
                }
            }
        }
    }
}
