using System.Windows;

namespace UI.Dialogs
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
