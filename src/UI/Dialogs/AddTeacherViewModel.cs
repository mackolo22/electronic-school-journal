﻿using ApplicationCore.Exceptions.AzureStorage;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;

namespace UI.Dialogs
{
    public class AddTeacherViewModel : AddPersonViewModel
    {
        private readonly IPersonService _personService;

        public AddTeacherViewModel(
            ILoginService loginService,
            IPersonService personService) : base(loginService)
        {
            _personService = personService;
        }

        public Teacher Teacher { get; set; }

        protected override async void ExecuteSaveChanges(object parameter)
        {
            Password = _loginService.GeneratePassword();

            try
            {
                Teacher = await _personService.AddTeacherAsync(FirstName, LastName, Login, Password);
            }
            catch (TableException ex)
            {
                // TODO: dodać w MessageBoxHelper boxa do wyjątków i odpalać go.
            }

            base.ExecuteSaveChanges(parameter);
        }
    }
}