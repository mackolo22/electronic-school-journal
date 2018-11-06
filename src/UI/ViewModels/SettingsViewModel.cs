using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using UI.Helpers;

namespace UI.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private const string EditButtonContent = "Edytuj";
        private const string SaveButtonContent = "Zapisz";

        private readonly IUsersRepository _usersRepository;
        private readonly ILoginService _loginService;
        private readonly IApplicationSettingsService _appSettingsService;
        private readonly LongRunningOperationHelper _longRunningOperationHelper;
        private User _user;

        public SettingsViewModel(
            IUsersRepository usersRepository,
            ILoginService loginService,
            IApplicationSettingsService appSettingsService,
            LongRunningOperationHelper longRunningOperationHelper)
        {
            _usersRepository = usersRepository;
            _loginService = loginService;
            _appSettingsService = appSettingsService;
            _longRunningOperationHelper = longRunningOperationHelper;
        }

        public string UserType { get; set; }
        public User User
        {
            get => _user;
            set
            {
                _user = value;
                Login = User.Login;
                Email = User.Email;
                OnPropertyChanged(nameof(User));
                OnPropertyChanged(nameof(Login));
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Login { get; set; }
        public string Email { get; set; }
        public string EditOrSaveButtonContent { get; set; } = EditButtonContent;
        public bool IsEditMode { get; set; }
        public bool IsChangePasswordMode { get; set; }

        public RelayCommand EditOrSaveChangesCommand => new RelayCommand(async (parameter) => await ExecuteEditOrSaveChangesAsync(parameter), () => true);
        private async Task ExecuteEditOrSaveChangesAsync(object parameter)
        {
            IsEditMode = !IsEditMode;
            if (!IsEditMode)
            {
                EditOrSaveButtonContent = EditButtonContent;
                IsChangePasswordMode = false;

                if (String.IsNullOrWhiteSpace(Login))
                {
                    MessageBoxHelper.ShowErrorMessageBox("Login nie może być pusty!");
                    Login = User.Login;
                    OnPropertyChanged(nameof(Login));
                    ChangeViewState();
                    return;
                }
                else
                {
                    User.Login = Login;
                }

                if (String.IsNullOrWhiteSpace(Email))
                {
                    MessageBoxHelper.ShowErrorMessageBox("E-mail nie może być pusty!");
                    Email = User.Email;
                    OnPropertyChanged(nameof(Email));
                    ChangeViewState();
                    return;
                }
                else
                {
                    User.Email = Email;
                }

                await _longRunningOperationHelper.ProceedLongRunningOperationAsync(async () =>
                {
                    var passwordBox = parameter as PasswordBox;
                    string hashedPassword = _loginService.HashPassword(passwordBox.Password);
                    if (!String.IsNullOrWhiteSpace(hashedPassword))
                    {
                        User.HashedPassword = hashedPassword;
                    }

                    await _usersRepository.InsertOrReplaceAsync(User);
                    _appSettingsService.SaveLoggedUserDataInRegistry(UserType, User);
                });
            }
            else
            {
                EditOrSaveButtonContent = SaveButtonContent;
            }

            ChangeViewState();
        }

        private void ChangeViewState()
        {
            OnPropertyChanged(nameof(EditOrSaveButtonContent));
            OnPropertyChanged(nameof(IsEditMode));
            OnPropertyChanged(nameof(IsChangePasswordMode));
        }

        public RelayCommand ChangePasswordCommand => new RelayCommand(ExecuteChangePassword, () => true);
        private void ExecuteChangePassword(object parameter)
        {
            IsChangePasswordMode = true;
            OnPropertyChanged(nameof(IsChangePasswordMode));
        }
    }
}
