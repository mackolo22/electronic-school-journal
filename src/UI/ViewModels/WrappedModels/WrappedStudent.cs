using ApplicationCore.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UI.ViewModels.WrappedModels
{
    public class WrappedStudent : BindableObject
    {
        private ObservableCollection<WrappedGrade> _grades;
        private string _average;
        private bool _presenceInSelectedDay;

        public long? Id { get; set; }
        public int OrdinalNumber { get; set; }
        public string FullName { get; set; }

        public ObservableCollection<WrappedGrade> Grades
        {
            get => _grades;
            set
            {
                _grades = value;
                OnPropertyChanged(nameof(Grades));
            }
        }

        public IDictionary<string, ObservableCollection<WrappedGrade>> AllGrades { get; set; }
        public string Average
        {
            get => _average;
            set
            {
                _average = value;
                OnPropertyChanged(nameof(Average));
            }
        }

        public ObservableCollection<Attendance> Attendances { get; set; }
        public bool PresenceInSelectedDay
        {
            get => _presenceInSelectedDay;
            set
            {
                _presenceInSelectedDay = value;
                OnPropertyChanged(nameof(PresenceInSelectedDay));
            }
        }
    }
}
