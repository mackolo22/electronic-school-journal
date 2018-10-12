using System.Windows;
using UI.ViewModels;
using UI.Views;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Window _window;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            UnityConfiguration.ConfigureUnityContainer();

            _window = new MainWindow()
            {
                DataContext = UnityConfiguration.Resolve<MainViewModel>()
            };

            _window.Show();
        }

        public static void ExitApplication()
        {
            _window.Close();
        }
    }
}
