using System.Windows;
using UI.ViewModels;

namespace UI.Views
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
