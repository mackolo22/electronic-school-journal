using System;
using System.Collections.Generic;
using System.Windows;
using UI.ViewModels;

namespace UI.Dialogs
{
    public class AddGradeViewModel : ViewModelBase
    {
        public List<double> AvailableGrades { get => new List<double> { 1, 2, 3, 4, 5, 6 }; }
        public double Grade { get; set; }
        public string Comment { get; set; }
        public DateTime LastModificationDate { get; set; }
        public bool ChangesSaved { get; set; }

        public RelayCommand SaveChangesCommand => new RelayCommand(ExecuteSaveChanges, () => true);
        private void ExecuteSaveChanges(object parameter)
        {
            ChangesSaved = true;

            if (parameter is Window window)
            {
                window.Close();
            }
        }
    }
}
