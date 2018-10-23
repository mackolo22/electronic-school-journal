using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for ShowGradeDialog.xaml
    /// </summary>
    public partial class ShowGradeDialog : Window
    {
        public ShowGradeDialog(ShowGradeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
