using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using UI.Views;

namespace UI.ViewModels
{
    public class AddStudentViewModel : AddUserViewModel
    {
        private readonly IUniqueIDGenerator _uniqueIDGenerator;

        public AddStudentViewModel(ILoginService loginService, IUniqueIDGenerator uniqueIDGenerator) : base(loginService)
        {
            _uniqueIDGenerator = uniqueIDGenerator;
        }

        public Parent Parent { get; set; }
        public bool ParentAdded { get; set; }

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
                    Login = viewModel.Login,
                    Email = viewModel.Email,
                    Password = viewModel.Password,
                    HashedPassword = viewModel.HashedPassword
                };

                ParentAdded = true;
                OnPropertyChanged(nameof(ParentAdded));
            }
        }

        protected override void ExecuteSaveChanges(object parameter)
        {
            Password = _loginService.GeneratePassword();
            HashedPassword = _loginService.HashPassword(Password);
            base.ExecuteSaveChanges(parameter);
        }
    }
}
