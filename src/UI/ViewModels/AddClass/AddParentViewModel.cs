﻿using ApplicationCore.Interfaces;
using ApplicationCore.Models;

namespace UI.ViewModels
{
    public class AddParentViewModel : AddPersonViewModel
    {
        private readonly IPersonService _personService;

        public AddParentViewModel(
            ILoginService loginService,
            IPersonService personService) : base(loginService)
        {
            _personService = personService;
        }

        public Parent Parent { get; set; }

        protected override void ExecuteSaveChanges(object parameter)
        {
            Password = _loginService.GeneratePassword();
            HashedPassword = _loginService.HashPassword(Password);
            base.ExecuteSaveChanges(parameter);
        }
    }
}