using System.Windows;

namespace UI.Dialogs
{
    /// <summary>
    /// Interaction logic for AddGradeDialog.xaml
    /// </summary>
    public partial class AddGradeDialog : Window
    {
        public AddGradeDialog(AddGradeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
