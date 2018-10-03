using ApplicationCore.Interfaces;
using Infrastructure.Data.AzureStorage.Tables;
using System.Windows;
using UI.ViewModels;
using UI.Views;
using Unity;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            UnityConfiguration.ConfigureUnityContainer();

            var window = new MainWindow()
            {
                DataContext = UnityConfiguration.Resolve<MainViewModel>()
            };

            window.Show();
        }
    }
}
