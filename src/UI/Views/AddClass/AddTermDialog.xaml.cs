using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for AddTermDialog.xaml
    /// </summary>
    public partial class AddTermDialog : Window
    {
        public AddTermDialog(AddTermViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
