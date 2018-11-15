using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UI.Helpers;
using UI.Views;

namespace UI.ViewModels
{
    public class LoginSecondStepViewModel : BaseViewModel
    {
        private Window _window;
        private LoginFirstStepViewModel _rootViewModel;
        private readonly ILoginService _loginService;
        private readonly IApplicationSettingsService _appSettingsService;
        private readonly LongRunningOperationHelper _longRunningOperationHelper;

        private static readonly Dictionary<string, string> UserTypesWithPolishNames = new Dictionary<string, string>
        {
            { "Administrator",  "administrator" },
            { "Student",        "uczeń" },
            { "Teacher",        "nauczyciel" },
            { "Parent",         "rodzic" }
        };

        public LoginSecondStepViewModel(
            ILoginService loginService,
            IApplicationSettingsService appSettingsService,
            LongRunningOperationHelper longRunningOperationHelper)
        {
            _loginService = loginService;
            _appSettingsService = appSettingsService;
            _longRunningOperationHelper = longRunningOperationHelper;
        }

        public LoginFirstStepViewModel ParentViewModel
        {
            get => _rootViewModel;
            set
            {
                _rootViewModel = value;
                string polishUserType = UserTypesWithPolishNames[value.UserType];
                Title = $"Logowanie jako {polishUserType}";
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Title { get; set; }
        public string Login { get; set; }

        public RelayCommand LoadedCommand => new RelayCommand(ExecuteLoaded, () => true);
        private void ExecuteLoaded(object parameter)
        {
            if (parameter is Window window)
            {
                _window = window;
            }
        }

        public RelayCommand LoginCommand => new RelayCommand(async (parameter) => await ExecuteLoginAsync(parameter), () => true);
        private async Task ExecuteLoginAsync(object parameter)
        {
            if (String.IsNullOrWhiteSpace(Login))
            {
                MessageBoxHelper.ShowErrorMessageBox("Podaj login.");
                return;
            }

            var passwordBox = parameter as PasswordBox;
            if (String.IsNullOrWhiteSpace(passwordBox.Password))
            {
                MessageBoxHelper.ShowErrorMessageBox("Podaj hasło.");
                return;
            }

            User user = null;
            await _longRunningOperationHelper.ProceedLongRunningOperationAsync(async () =>
            {
                user = await _loginService.LoginUserAsync(ParentViewModel.UserType, Login, passwordBox.Password);
            });

            bool loggedIn = (user != null);
            if (loggedIn && ParentViewModel.UserType != "Administrator")
            {
                _appSettingsService.SaveLoggedUserDataInRegistry(ParentViewModel.UserType, user);
            }

            ParentViewModel.User = user;
            ParentViewModel.LoggedIn = loggedIn;
            if (loggedIn)
            {
                _window.Close();
            }
            else
            {
                MessageBoxHelper.ShowErrorMessageBox("Niepoprawne dane logowania!");
            }
        }

        public RelayCommand RecoverPasswordCommand => new RelayCommand(ExecuteRecoverPassword, () => true);
        private void ExecuteRecoverPassword(object parameter)
        {
            var viewModel = UnityConfiguration.Resolve<RecoverPasswordViewModel>();
            viewModel.UserType = ParentViewModel.UserType;
            var dialog = new RecoverPasswordDialog(viewModel);
            dialog.ShowDialog();
        }

        public RelayCommand LoginInOfflineModeCommand => new RelayCommand(ExecuteLoginInOfflineMode, () => true);
        private void ExecuteLoginInOfflineMode(object parameter)
        {
            var passwordBox = parameter as PasswordBox;

            if (String.IsNullOrWhiteSpace(Login))
            {
                MessageBoxHelper.ShowErrorMessageBox("Podaj login.");
                return;
            }

            if (String.IsNullOrWhiteSpace(passwordBox.Password))
            {
                MessageBoxHelper.ShowErrorMessageBox("Podaj hasło.");
                return;
            }

            User user = _appSettingsService.GetLoggedUserDataFromRegistry(ParentViewModel.UserType);
            if (user != null)
            {
                bool loggedIn = _loginService.LoginUserInOfflineMode(user, Login, passwordBox.Password);
                ParentViewModel.IsOfflineMode = true;
                ParentViewModel.LoggedIn = loggedIn;
                ParentViewModel.User = user;
                if (loggedIn)
                {
                    _window.Close();
                }
                else
                {
                    MessageBoxHelper.ShowErrorMessageBox("Niepoprawne dane logowania!");
                }
            }
            else
            {
                MessageBoxHelper.ShowErrorMessageBox("W systemie nie zapisano żadnych danych logowania.");
            }
        }
    }
}
