using ApplicationCore.Exceptions;
using ApplicationCore.Exceptions.AzureStorage;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UI.Helpers;

namespace UI.ViewModels
{
    public class TeachersViewModel : BaseViewModel
    {
        private readonly ILoginService _loginService;
        private readonly IUsersRepository _usersRepository;
        private readonly IUniqueIDGenerator _uniqueIDGenerator;
        private readonly IMailingService _mailingService;
        private readonly LongRunningOperationHelper _longRunningOperationHelper;

        public TeachersViewModel(
            ILoginService loginService,
            IUsersRepository usersRepository,
            IUniqueIDGenerator uniqueIDGenerator,
            IMailingService mailingService,
            LongRunningOperationHelper longRunningOperationHelper)
        {
            _loginService = loginService;
            _usersRepository = usersRepository;
            _uniqueIDGenerator = uniqueIDGenerator;
            _mailingService = mailingService;
            _longRunningOperationHelper = longRunningOperationHelper;
        }

        public Administrator Administrator { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public ObservableCollection<Teacher> AllTeachers { get; set; }

        public RelayCommand LoadedCommand => new RelayCommand(async (parameter) => await ExecuteLoadedAsync(parameter), () => true);
        private async Task ExecuteLoadedAsync(object parameter)
        {
            await _longRunningOperationHelper.ProceedLongRunningOperationAsync(async () =>
            {
                var users = await _usersRepository.GetAllAsync(nameof(Teacher));
                var teachers = users as List<Teacher>;
                AllTeachers = new ObservableCollection<Teacher>(teachers);
                OnPropertyChanged(nameof(AllTeachers));
            });
        }

        public RelayCommand GenerateAutoLoginCommand => new RelayCommand(ExecuteGenerateAutoLogin, () => true);
        private void ExecuteGenerateAutoLogin(object obj)
        {
            try
            {
                Login = _loginService.GenerateLogin(FirstName, LastName);
                OnPropertyChanged(nameof(Login));
            }
            catch (LoginException exception)
            {
                MessageBoxHelper.ShowErrorMessageBox(exception.Message);
            }
        }

        public RelayCommand AddTeacherCommand => new RelayCommand(async (parameter) => await ExeucteAddTeacherAsync(parameter), () => true);
        private async Task ExeucteAddTeacherAsync(object parameter)
        {
            if (String.IsNullOrWhiteSpace(FirstName) || String.IsNullOrWhiteSpace(LastName) || String.IsNullOrWhiteSpace(Login) || String.IsNullOrWhiteSpace(Email))
            {
                MessageBoxHelper.ShowErrorMessageBox("Wypełnij wszystkie pola.");
                return;
            }

            try
            {
                await _longRunningOperationHelper.ProceedLongRunningOperationAsync(async () =>
                {
                    string password = _loginService.GeneratePassword();
                    string hashedPassword = _loginService.HashPassword(password);
                    long id = _uniqueIDGenerator.GetNextIdForUser();
                    var teacher = new Teacher(id)
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        Login = Login,
                        Email = Email,
                        Password = password,
                        HashedPassword = hashedPassword
                    };

                    await _usersRepository.InsertOrReplaceAsync(teacher);
                    await _mailingService.SendEmailWithLoginAndPasswordAsync(teacher, Administrator);
                    AllTeachers.Add(teacher);
                    OnPropertyChanged(nameof(AllTeachers));
                });

                MessageBoxHelper.ShowMessageBox("Nauczyciel dodany do systemu.");
                ClearAllFields();
            }
            catch (TableException ex)
            {
                MessageBoxHelper.ShowErrorMessageBox(ex.Message);
            }
        }

        private void ClearAllFields()
        {
            FirstName = String.Empty;
            LastName = String.Empty;
            Login = String.Empty;
            Email = String.Empty;

            OnPropertyChanged(nameof(FirstName));
            OnPropertyChanged(nameof(LastName));
            OnPropertyChanged(nameof(Login));
            OnPropertyChanged(nameof(Email));
        }
    }
}
