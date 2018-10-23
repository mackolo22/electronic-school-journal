using UI.ViewModels.WrappedModels;

namespace UI.ViewModels
{
    public class ShowGradeViewModel : ViewModelBase
    {
        public WrappedGrade Grade { get; set; }
        public string Value => $"Ocena: {Grade.Value}";
        public string LastModified => $"Ostatnia zmiana: {Grade.LastModificationDate.ToShortDateString()}, godz. {Grade.LastModificationDate.ToShortTimeString()}";
        public string Comment => Grade.Comment;
    }
}
