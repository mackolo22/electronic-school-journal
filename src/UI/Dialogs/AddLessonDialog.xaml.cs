using System.Windows;

namespace UI.Dialogs
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
