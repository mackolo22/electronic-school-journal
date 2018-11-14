using ApplicationCore.Exceptions.AzureStorage;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UI.Helpers;
using UI.Views;

namespace UI.ViewModels
{
    public class CreateClassViewModel : BaseViewModel
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IClassesRepository _classesRepository;
        private readonly IUniqueIDGenerator _uniqueIDGenerator;
        private readonly IMailingService _mailingService;
        private readonly LongRunningOperationHelper _longRunningOperationHelper;

        public CreateClassViewModel(
            IUsersRepository usersRepository,
            IClassesRepository classesRepository,
            IUniqueIDGenerator uniqueIDGenerator,
            IMailingService mailingService,
            LongRunningOperationHelper longRunningOperationHelper)
        {
            _usersRepository = usersRepository;
            _classesRepository = classesRepository;
            _uniqueIDGenerator = uniqueIDGenerator;
            _mailingService = mailingService;
            _longRunningOperationHelper = longRunningOperationHelper;
        }

        public List<int> AvailableClassNumbers => new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
        public List<string> AvailableClassLetters => new List<string> { "A", "B", "C", "D" };

        public Administrator Administrator { get; set; }
        public ObservableCollection<Teacher> AllTeachers { get; set; }
        public Teacher Educator { get; set; }
        public int ClassNumber { get; set; }
        public string ClassLetter { get; set; }
        public ObservableCollection<Student> Students { get; set; }

        public RelayCommand LoadedCommand => new RelayCommand(async (parameter) => await ExecuteLoadedAsync(parameter), () => true);
        private async Task ExecuteLoadedAsync(object parameter)
        {
            await _longRunningOperationHelper.ProceedLongRunningOperationAsync(async () =>
            {
                var users = await _usersRepository.GetAllAsync(nameof(Teacher));
                var teachers = users as List<Teacher>;
                AllTeachers = new ObservableCollection<Teacher>(teachers);
                OnPropertyChanged(nameof(AllTeachers));
                Students = new ObservableCollection<Student>();
            });
        }

        public RelayCommand AddStudentCommand => new RelayCommand(ExecuteAddStudent, () => true);
        private void ExecuteAddStudent(object parameter)
        {
            var viewModel = UnityConfiguration.Resolve<AddStudentViewModel>();
            var dialog = new AddStudentDialog(viewModel);
            dialog.ShowDialog();

            if (viewModel.ChangesSaved)
            {
                long studentId = _uniqueIDGenerator.GetNextIdForUser();
                Student student = new Student(studentId)
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    ParentId = viewModel.Parent?.Id,
                    Parent = viewModel.Parent,
                    Login = viewModel.Login,
                    Email = viewModel.Email,
                    Password = viewModel.Password,
                    HashedPassword = viewModel.HashedPassword
                };

                if (student.Parent != null)
                {
                    student.Parent.ChildId = student.Id;
                }

                Students.Add(student);
                OnPropertyChanged(nameof(Students));
            }
        }

        public RelayCommand SaveChangesCommand => new RelayCommand(async (parameter) => await ExecuteSaveChangesAsync(parameter), () => true);
        private async Task ExecuteSaveChangesAsync(object parameter)
        {
            // TODO: sprawdzenie czy pola są podane
            await _longRunningOperationHelper.ProceedLongRunningOperationAsync(async () =>
            {
                var studentsClass = new StudentsClass(ClassNumber, ClassLetter)
                {
                    EducatorId = Educator?.Id,
                    Educator = Educator,
                    Students = Students
                };

                Educator.ClassId = studentsClass.FullName;

                try
                {
                    await _usersRepository.InsertOrReplaceAsync(Educator);
                    foreach (var student in Students)
                    {
                        student.ClassId = studentsClass.FullName;
                        student.Class = studentsClass;
                        await _usersRepository.InsertOrReplaceAsync(student);
                        await _mailingService.SendEmailWithLoginAndPasswordAsync(student, Administrator);

                        if (student.Parent != null)
                        {
                            student.Parent.ChildClassId = studentsClass.FullName;
                            await _usersRepository.InsertOrReplaceAsync(student.Parent);
                            await _mailingService.SendEmailWithLoginAndPasswordAsync(student.Parent, Administrator);
                        }
                    }

                    await _classesRepository.InsertOrReplaceAsync(studentsClass);
                }
                catch (TableException)
                {
                    // TODO: wycofanie wszystkich zmian
                }
            });

            MessageBoxHelper.ShowMessageBox($"Utworzono klasę {ClassNumber}{ClassLetter} wraz z kontami użytkowników.");
        }
    }
}
