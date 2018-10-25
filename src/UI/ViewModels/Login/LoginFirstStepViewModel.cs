using ApplicationCore.Models;
using System.Windows;
using UI.Views;

namespace UI.ViewModels
{
    public class LoginFirstStepViewModel : BaseViewModel
    {
        private Window _window;
        private bool _closeApplicationOnWindowClosed = true;

        public string UserType { get; set; }
        public Person Person { get; set; }
        public Student Student { get; set; }
        public Teacher Teacher { get; set; }
        public Parent Parent { get; set; }
        public Administrator Administrator { get; set; }
        public bool LoggedIn { get; set; }

        public RelayCommand LoadedCommand => new RelayCommand(ExecuteLoaded, () => true);
        private void ExecuteLoaded(object parameter)
        {
            if (parameter is Window window)
            {
                _window = window;
            }
        }

        public RelayCommand ClosedCommand => new RelayCommand(ExecuteClosed, () => true);
        private void ExecuteClosed(object parameter)
        {
            if (_closeApplicationOnWindowClosed)
            {
                App.ExitApplication();
            }
        }

        public RelayCommand UserTypeSelectedCommand => new RelayCommand(ExecuteUserTypeSelected, () => true);
        private void ExecuteUserTypeSelected(object parameter)
        {
            string userType = parameter as string;
            UserType = userType;
            var viewModel = UnityConfiguration.Resolve<LoginSecondStepViewModel>();
            viewModel.RootViewModel = this;
            var dialog = new LoginSecondStepDialog(viewModel);
            dialog.ShowDialog();
            if (LoggedIn)
            {
                _closeApplicationOnWindowClosed = false;
                _window.Close();
            }
        }
    }
}
