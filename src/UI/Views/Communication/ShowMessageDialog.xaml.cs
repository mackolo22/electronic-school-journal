using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for ShowMessageDialog.xaml
    /// </summary>
    public partial class ShowMessageDialog : Window
    {
        public ShowMessageDialog(ShowMessageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
