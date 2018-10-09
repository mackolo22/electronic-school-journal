using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UI.ViewModels;

namespace UI.Dialogs
{
    public class AddTermViewModel : ViewModelBase
    {
        private Day _day;
        private TimeSpan _time;
        private string _hour;
        private string _minutes;

        public IEnumerable<Day> Days
        {
            get
            {
                return Enum.GetValues(typeof(Day)).Cast<Day>();
            }
        }

        public Day Day
        {
            get => _day;
            set
            {
                _day = value;
                OnPropertyChanged(nameof(Day));
            }
        }

        public string[] AvailableHours => new string[] { "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17" };

        public string Hour
        {
            get => _hour;
            set
            {
                _hour = value;
                OnPropertyChanged(nameof(Hour));
            }
        }

        public string[] AvailableMinutes => new string[] { "00", "05", "10", "15", "20", "25", "30", "35", "40", "45", "50", "55" };

        public string Minutes
        {
            get => _minutes;
            set
            {
                _minutes = value;
                OnPropertyChanged(nameof(Minutes));
            }
        }

        public RelayCommand SaveChangesCommand => new RelayCommand(ExecuteSaveChanges, () => true);
        protected virtual void ExecuteSaveChanges(object parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }
    }
}
