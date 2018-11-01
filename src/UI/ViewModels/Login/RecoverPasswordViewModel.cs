using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UI.Helpers;
using UI.Views;

namespace UI.ViewModels
{
    public class RecoverPasswordViewModel : BaseViewModel
    {
        private readonly ITableStorageRepository _repository;
        private readonly IMailingService _mailingService;
        private readonly ILoginService _loginService;
        private Window _window;
        private int _recoveryCode;
        private Student _student;
        private Teacher _teacher;
        private Parent _parent;
        private Administrator _administrator;

        public RecoverPasswordViewModel(
            ITableStorageRepository repository,
            IMailingService mailingService,
            ILoginService loginService)
        {
            _repository = repository;
            _mailingService = mailingService;
            _loginService = loginService;
        }

        public string UserType { get; set; }
        public string Email { get; set; }
        public bool RecoveryCodeSent { get; set; }
        public string RecoveryCode { get; set; }

        public RelayCommand LoadedCommand => new RelayCommand(ExecuteLoaded, () => true);
        private void ExecuteLoaded(object parameter)
        {
            if (parameter is Window window)
            {
                _window = window;
            }
        }

        public RelayCommand SendEmailWithCodeCommand => new RelayCommand(async (parameter) => await ExecuteSendEmailWithCodeAsync(parameter), () => true);
        private async Task ExecuteSendEmailWithCodeAsync(object parameter)
        {
            var dialog = new OperationInProgressDialog();
            dialog.Show();

            switch (UserType)
            {
                case "Student":
                    var students = await _repository.GetAllByPropertyAsync<Student>(nameof(Student), "Email", Email);
                    _student = students.FirstOrDefault();
                    break;

                case "Parent":
                    var parents = await _repository.GetAllByPropertyAsync<Parent>(nameof(Parent), "Email", Email);
                    _parent = parents.FirstOrDefault();
                    break;

                case "Teacher":
                    var teachers = await _repository.GetAllByPropertyAsync<Teacher>(nameof(Teacher), "Email", Email);
                    _teacher = teachers.FirstOrDefault();
                    break;

                case "Administrator":
                    var administrators = await _repository.GetAllByPropertyAsync<Administrator>(nameof(Administrator), "Email", Email);
                    _administrator = administrators.FirstOrDefault();
                    break;
            }

            if (_student == null && _parent == null && _teacher == null && _administrator == null)
            {
                dialog.Close();
                MessageBoxHelper.ShowErrorMessageBox("Podany adres e-mail nie jest przypisany do żadnego użytkownika.");
                return;
            }

            var random = new Random();
            _recoveryCode = random.Next(10000, 90000);
            await _mailingService.SendEmailWithRecoveryCodeAsync(Email, _recoveryCode);

            dialog.Close();
            MessageBoxHelper.ShowMessageBox("Na podany adres e-mail został wysłany kod resetujący hasło.");
            RecoveryCodeSent = true;
            OnPropertyChanged(nameof(RecoveryCodeSent));
        }

        public RelayCommand RecoverPasswordCommand => new RelayCommand(async (parameter) => await ExecuteRecoverPasswordAsync(parameter), () => true);
        private async Task ExecuteRecoverPasswordAsync(object parameter)
        {
            if (String.IsNullOrWhiteSpace(RecoveryCode))
            {
                MessageBoxHelper.ShowErrorMessageBox("Wprowadź kod resetujący hasło otrzymany w wiadomości e-mail.");
                return;
            }

            Int32.TryParse(RecoveryCode, out int userCode);
            if (userCode == _recoveryCode)
            {
                var dialog = new OperationInProgressDialog();
                dialog.Show();

                string password = _loginService.GeneratePassword();
                string hashedPassword = _loginService.HashPassword(password);
                switch (UserType)
                {
                    case "Student":
                        _student.HashedPassword = hashedPassword;
                        await _repository.InsertOrReplaceAsync(_student);
                        break;

                    case "Parent":
                        _parent.HashedPassword = hashedPassword;
                        await _repository.InsertOrReplaceAsync(_parent);
                        break;

                    case "Teacher":
                        _teacher.HashedPassword = hashedPassword;
                        await _repository.InsertOrReplaceAsync(_teacher);
                        break;

                    case "Administrator":
                        _administrator.HashedPassword = hashedPassword;
                        await _repository.InsertOrReplaceAsync(_administrator);
                        break;
                }

                await _mailingService.SendEmailWithNewPasswordAsync(Email, password);

                dialog.Close();
                MessageBoxHelper.ShowMessageBox("Twoje hasło zostało zresetowane. Na podany adres e-mail zostało wysłane nowe hasło.");
                _window.Close();
            }
            else
            {
                MessageBoxHelper.ShowErrorMessageBox("Wprowadzono niepoprawny kod resetujący hasło.");
            }
        }
    }
}
