using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for AddStudentDialog.xaml
    /// </summary>
    public partial class AddStudentDialog : Window
    {
        public AddStudentDialog(AddStudentViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
