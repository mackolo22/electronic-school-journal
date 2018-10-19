using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for ShowLessonDialog.xaml
    /// </summary>
    public partial class ShowLessonDialog : Window
    {
        public ShowLessonDialog(ShowLessonViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
