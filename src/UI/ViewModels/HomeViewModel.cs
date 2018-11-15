namespace UI.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private string _loginMode;
        public string LoginMode
        {
            get => _loginMode;
            set
            {
                _loginMode = $"Jesteś zalogowany\nw trybie {value}";
                OnPropertyChanged(nameof(LoginMode));
            }
        }
    }
}
