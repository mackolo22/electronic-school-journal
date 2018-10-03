using System.Windows;

namespace UI.Dialogs
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
