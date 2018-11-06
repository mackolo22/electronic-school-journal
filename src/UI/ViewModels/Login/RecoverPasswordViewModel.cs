using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UI.Helpers;

namespace UI.ViewModels
{
    public class RecoverPasswordViewModel : BaseViewModel
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IMailingService _mailingService;
        private readonly ILoginService _loginService;
        private readonly LongRunningOperationHelper _longRunningOperationHelper;
        private Window _window;
        private int _recoveryCode;
        private User _user;

        public RecoverPasswordViewModel(
            IUsersRepository usersRepository,
            IMailingService mailingService,
            ILoginService loginService,
            LongRunningOperationHelper longRunningOperationHelper)
        {
            _usersRepository = usersRepository;
            _mailingService = mailingService;
            _loginService = loginService;
            _longRunningOperationHelper = longRunningOperationHelper;
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
            await _longRunningOperationHelper.ProceedLongRunningOperationAsync(async () =>
            {
                var users = await _usersRepository.GetAllByPropertyAsync(nameof(Student), "Email", Email);
                _user = users.FirstOrDefault();
                if (_user != null)
                {
                    var random = new Random();
                    _recoveryCode = random.Next(10000, 90000);
                    await _mailingService.SendEmailWithRecoveryCodeAsync(Email, _recoveryCode);
                }
            });

            if (_user != null)
            {
                MessageBoxHelper.ShowMessageBox("Na podany adres e-mail został wysłany kod resetujący hasło.");
                RecoveryCodeSent = true;
                OnPropertyChanged(nameof(RecoveryCodeSent));
            }
            else
            {
                MessageBoxHelper.ShowErrorMessageBox("Podany adres e-mail nie jest przypisany do żadnego użytkownika.");
            }
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
                await _longRunningOperationHelper.ProceedLongRunningOperationAsync(async () =>
                {
                    string password = _loginService.GeneratePassword();
                    string hashedPassword = _loginService.HashPassword(password);
                    _user.HashedPassword = hashedPassword;
                    await _usersRepository.InsertOrReplaceAsync(_user);
                    await _mailingService.SendEmailWithNewPasswordAsync(Email, password);

                });

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
