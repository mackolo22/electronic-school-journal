using ApplicationCore.Interfaces;

namespace UI.Dialogs
{
    public class AddTeacherViewModel : AddPersonViewModel
    {
        public AddTeacherViewModel(ILoginService loginService) : base(loginService) { }
    }
}
