using ApplicationCore.Interfaces;
using ApplicationCore.Models;

namespace UI.ViewModels
{
    public class AddParentViewModel : AddUserViewModel
    {
        public AddParentViewModel(ILoginService loginService) : base(loginService) { }

        public Parent Parent { get; set; }

        protected override void ExecuteSaveChanges(object parameter)
        {
            Password = _loginService.GeneratePassword();
            HashedPassword = _loginService.HashPassword(Password);
            base.ExecuteSaveChanges(parameter);
        }
    }
}
