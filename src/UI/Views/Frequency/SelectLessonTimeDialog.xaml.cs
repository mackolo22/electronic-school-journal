using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for SelectLessonTimeDialog.xaml
    /// </summary>
    public partial class SelectLessonTimeDialog : Window
    {
        public SelectLessonTimeDialog(SelectLessonTimeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
