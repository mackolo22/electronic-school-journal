using System.Collections.ObjectModel;

namespace UI.ViewModels.WrappedModels
{
    public class WrappedSubject
    {
        public string Name { get; set; }
        public ObservableCollection<WrappedGrade> Grades { get; set; }
        public double Average { get; set; }
    }
}
