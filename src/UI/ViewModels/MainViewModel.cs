using System;
using UI.Dialogs;

namespace UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _viewModel;
        private string _loggedAs;

        public MainViewModel()
        {
            ViewModel = UnityConfiguration.Resolve<HomeViewModel>();
        }

        public ViewModelBase ViewModel
        {
            get => _viewModel;
            set
            {
                _viewModel = value;
                OnPropertyChanged(nameof(ViewModel));
            }
        }

        public string LoggedAs
        {
            get => _loggedAs;
            set
            {
                _loggedAs = value;
                OnPropertyChanged(nameof(LoggedAs));
            }
        }

        public RelayCommand LoadedCommand => new RelayCommand(ExecuteLoaded, () => true);
        private void ExecuteLoaded(object parameter)
        {
            var viewModel = UnityConfiguration.Resolve<LoginViewModel>();
            var dialog = new LoginDialog(viewModel);
            dialog.ShowDialog();
        }

        public RelayCommand ChangeViewCommand
        {
            get => new RelayCommand(ExecuteChangeView, () => true);
        }

        private void ExecuteChangeView(object parameter)
        {
            string viewName = parameter as string;
            if (!String.IsNullOrWhiteSpace(viewName))
            {
                switch (viewName)
                {
                    case "HomeView":
                        if (!(ViewModel is HomeViewModel))
                        {
                            ViewModel = UnityConfiguration.Resolve<HomeViewModel>();
                        }
                        break;

                    case "AddClassView":
                        if (!(ViewModel is AddClassViewModel))
                        {
                            ViewModel = UnityConfiguration.Resolve<AddClassViewModel>();
                        }
                        break;
                }
            }
        }
    }
}
