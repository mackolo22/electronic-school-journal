using System.Windows;

namespace UI.Dialogs
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
