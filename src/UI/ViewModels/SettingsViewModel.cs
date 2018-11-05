using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using UI.Helpers;
using UI.Views;

namespace UI.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private const string EditButtonContent = "Edytuj";
        private const string SaveButtonContent = "Zapisz";
        private readonly ITableStorageRepository _repository;
        private readonly ILoginService _loginService;
        private Person _person;

        public SettingsViewModel(ITableStorageRepository repository, ILoginService loginService)
        {
            _repository = repository;
            _loginService = loginService;
        }

        public string UserType { get; set; }
        public Person Person
        {
            get => _person;
            set
            {
                _person = value;
                Login = Person.Login;
                Email = Person.Email;
                OnPropertyChanged(nameof(Person));
                OnPropertyChanged(nameof(Login));
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Login { get; set; }
        public string Email { get; set; }
        public Administrator Administrator { get; set; }
        public Student Student { get; set; }
        public Teacher Teacher { get; set; }
        public Parent Parent { get; set; }
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
                    Login = Person.Login;
                    OnPropertyChanged(nameof(Login));
                    ChangeViewState();
                    return;
                }
                else
                {
                    Person.Login = Login;
                }

                if (String.IsNullOrWhiteSpace(Email))
                {
                    MessageBoxHelper.ShowErrorMessageBox("E-mail nie może być pusty!");
                    Email = Person.Email;
                    OnPropertyChanged(nameof(Email));
                    ChangeViewState();
                    return;
                }
                else
                {
                    Person.Email = Email;
                }

                var dialog = new OperationInProgressDialog();
                dialog.Show();

                var passwordBox = parameter as PasswordBox;
                string hashedPassword = _loginService.HashPassword(passwordBox.Password);
                if (!String.IsNullOrWhiteSpace(hashedPassword))
                {
                    Person.HashedPassword = hashedPassword;
                }

                if (UserType == "Administrator")
                {
                    await _repository.InsertOrReplaceAsync(Administrator);
                }
                else if (UserType == "Student")
                {
                    await _repository.InsertOrReplaceAsync(Student);
                }
                else if (UserType == "Teacher")
                {
                    await _repository.InsertOrReplaceAsync(Teacher);
                }
                else if (UserType == "Parent")
                {
                    await _repository.InsertOrReplaceAsync(Parent);
                }

                dialog.Close();
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
