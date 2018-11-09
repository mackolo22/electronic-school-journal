using ApplicationCore.Interfaces;
using ApplicationCore.Models;

namespace UI.ViewModels
{
    public class AddParentViewModel : AddUserViewModel
    {
        private readonly IUserService _userService;

        public AddParentViewModel(
            ILoginService loginService,
            IUserService userService) : base(loginService)
        {
            _userService = userService;
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
  