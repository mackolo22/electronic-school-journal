using ApplicationCore.Enums;
using ApplicationCore.Models;

namespace UI.ViewModels.WrappedModels
{
    public class WrappedLesson : BindableObject
    {
        private Subject _subject;
        public Subject Subject
        {
            get => _subject;
            set
            {
                _subject = value;
                OnPropertyChanged(nameof(Subject));
            }
        }

        public long? TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public string Classroom { get; set; }
        public string ClassName { get; set; }
        public LessonTerm Term { get; set; }
    }
}
