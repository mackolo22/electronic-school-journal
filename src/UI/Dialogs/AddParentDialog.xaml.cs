using System.Windows;

namespace UI.Dialogs
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
