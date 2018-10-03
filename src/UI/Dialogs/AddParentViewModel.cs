using ApplicationCore.Interfaces;

namespace UI.Dialogs
{
    public class AddParentViewModel : AddPersonViewModel
    {
        public AddParentViewModel(ILoginService loginService) : base(loginService) { }
    }
}
