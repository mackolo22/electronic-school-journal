using ApplicationCore.Exceptions.AzureStorage;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System.Collections.Generic;
using UI.Views;

namespace UI.ViewModels
{
    public class AddTeacherViewModel : AddPersonViewModel
    {
        private readonly IPersonService _personService;

        public AddTeacherViewModel(ILoginService loginService, IPersonService personService) : base(loginService)
        {
            _personService = personService;
        }

        public Administrator Administrator { get; set; }
        public Teacher Teacher { get; set; }

        protected override async void ExecuteSaveChanges(object parameter)
        {
            Password = _loginService.GeneratePassword();
            HashedPassword = _loginService.HashPassword(Password);

            try
            {
                var dialog = new OperationInProgressDialog();
                dialog.Show();

                Teacher = await _personService.AddTeacherAsync(Administrator, FirstName, LastName, Login, Email, Password, HashedPassword);
                Teacher.Lessons = new List<Lesson>();

                dialog.Close();
            }
            catch (TableException)
            {
                // TODO: dodać w MessageBoxHelper boxa do wyjątków i odpalać go.
            }

            base.ExecuteSaveChanges(parameter);
        }
    }
}
