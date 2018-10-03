using System.Windows;

namespace UI.Dialogs
{
    /// <summary>
    /// Interaction logic for AddStudentDialog.xaml
    /// </summary>
    public partial class AddTeacherDialog : Window
    {
        public AddTeacherDialog(AddTeacherViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
