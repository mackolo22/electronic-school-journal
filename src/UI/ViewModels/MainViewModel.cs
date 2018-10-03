using System;
using System.Windows.Input;

namespace UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _viewModel;

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

        public ICommand ChangeViewCommand
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
                        ViewModel = UnityConfiguration.Resolve<HomeViewModel>();
                        break;

                    case "AddClassView":
                        ViewModel = UnityConfiguration.Resolve<AddClassViewModel>();
                        break;
                }
            }
        }
    }
}
