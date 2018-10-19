using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for AddLessonDialog.xaml
    /// </summary>
    public partial class AddLessonDialog : Window
    {
        public AddLessonDialog(AddLessonViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
