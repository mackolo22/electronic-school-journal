using ApplicationCore.Exceptions.AzureStorage;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System.Collections.Generic;
using UI.Helpers;

namespace UI.ViewModels
{
    public class AddTeacherViewModel : AddUserViewModel
    {
        private readonly IUserService _userService;
        private readonly LongRunningOperationHelper _longRunningOperationHelper;

        public AddTeacherViewModel(
            ILoginService loginService,
            IUserService userService,
            LongRunningOperationHelper longRunningOperationHelper) : base(loginService)
        {
            _userService = userService;
            _longRunningOperationHelper = longRunningOperationHelper;
        }

        public Administrator Administrator { get; set; }
        public Teacher Teacher { get; set; }

        protected override async void ExecuteSaveChanges(object parameter)
        {
            Password = _loginService.GeneratePassword();
            HashedPassword = _loginService.HashPassword(Password);

            try
            {
                await _longRunningOperationHelper.ProceedLongRunningOperationAsync(async () =>
                {
                    Teacher = await _userService.AddTeacherAsync(Administrator, FirstName, LastName, Login, Email, Password, HashedPassword);
                    Teacher.Lessons = new List<Lesson>();
                });
            }
            catch (TableException)
            {
                // TODO: dodać w MessageBoxHelper boxa do wyjątków i odpalać go.
            }

            base.ExecuteSaveChanges(parameter);
        }
    }
}
