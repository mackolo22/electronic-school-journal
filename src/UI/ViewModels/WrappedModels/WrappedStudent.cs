using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UI.ViewModels.WrappedModels
{
    public class WrappedStudent : BindableObject
    {
        private ObservableCollection<WrappedGrade> _grades;
        private string _average;

        public long? Id { get; set; }
        public int Number { get; set; }
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
    }
}
