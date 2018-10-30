using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace UI.ViewModels
{
    public class AddTermViewModel : BaseViewModel
    {
        private Day _day;

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

        public string[] AvailableHours => new string[] { "8:00 - 8:45", "8:55 - 9:40", "9:50 - 10:35", "10:50 - 11:35", "11:45 - 12:30", "12:40 - 13:25", "13:30 - 14:15", "14:20 - 15:05" };

        public string Time { get; set; }
        public int LessonNumber { get; set; }

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
