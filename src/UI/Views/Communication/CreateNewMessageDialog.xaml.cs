using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for CreateNewMessageDialog.xaml
    /// </summary>
    public partial class CreateNewMessageDialog : Window
    {
        public CreateNewMessageDialog(CreateNewMessageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
