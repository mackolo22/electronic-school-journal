using ApplicationCore.Models;
using System.Collections.Generic;
using System.Windows;

namespace UI.ViewModels
{
    public class SelectLessonTimeViewModel : BaseViewModel
    {
        public IEnumerable<LessonTerm> Terms { get; set; }
        public LessonTerm SelectedTerm { get; set; }

        public RelayCommand AcceptCommand => new RelayCommand(ExecuteAccept, () => true);
        private void ExecuteAccept(object parameter)
        {
            var window = parameter as Window;
            window.Close();
        }
    }
}
