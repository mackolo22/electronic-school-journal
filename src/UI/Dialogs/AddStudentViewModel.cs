using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using UI.ViewModels;

namespace UI.Dialogs
{
    public class AddStudentViewModel : AddPersonViewModel
    {
        private readonly IUniqueIDGenerator _uniqueIDGenerator;

        public AddStudentViewModel(ILoginService loginService, IUniqueIDGenerator uniqueIDGenerator) : base(loginService)
        {
            _uniqueIDGenerator = uniqueIDGenerator;
        }

        public Parent Parent { get; set; }

        public RelayCommand AddParentCommand => new RelayCommand(ExecuteAddParent, () => true);
        private void ExecuteAddParent(object parameter)
        {
            var viewModel = UnityConfiguration.Resolve<AddParentViewModel>();
            var dialog = new AddParentDialog(viewModel);
            dialog.ShowDialog();

            if (viewModel.ChangesSaved)
            {
                long id = _uniqueIDGenerator.GetNextId();
                Parent = new Parent(id)
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    User = new User
                    {
                        Login = viewModel.Login,
                        Password = viewModel.Password
                    }
                };
            }
        }
    }
}
