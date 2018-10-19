using System;
using System.Collections.Generic;
using System.Windows;

namespace UI.ViewModels
{
    public class AddGradeViewModel : ViewModelBase
    {
        private DateTime _lastModificationDate;

        public string Title { get; set; }
        public List<double> AvailableGrades { get => new List<double> { 1, 2, 3, 4, 5, 6 }; }
        public double Grade { get; set; }
        public string Comment { get; set; }

        public DateTime LastModificationDate
        {
            get => _lastModificationDate;
            set
            {
                _lastModificationDate = value;
                LastModificationDateSet = true;
                LastModified = $"Ostatnia zmiana: {value.ToShortDateString()}, godz. {value.ToShortTimeString()}";
                OnPropertyChanged(nameof(LastModificationDate));
                OnPropertyChanged(nameof(LastModificationDateSet));
                OnPropertyChanged(nameof(LastModified));
            }
        }

        public string LastModified { get; set; }
        public bool LastModificationDateSet { get; private set; }
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
