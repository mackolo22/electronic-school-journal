using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for AddStudentDialog.xaml
    /// </summary>
    public partial class AddParentDialog : Window
    {
        public AddParentDialog(AddParentViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
