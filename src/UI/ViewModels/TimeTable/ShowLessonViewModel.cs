namespace UI.ViewModels
{
    public class ShowLessonViewModel : BaseViewModel
    {
        private string _subject;
        private string _classroom;
        private string _teacherFullName;
        private string _className;

        public string Subject
        {
            get => _subject;
            set
            {
                _subject = $"Przedmiot: {value}";
                OnPropertyChanged(nameof(Subject));
            }
        }

        public string TeacherFullName
        {
            get => _teacherFullName;
            set
            {
                _teacherFullName = $"Nauczyciel: {value}";
                OnPropertyChanged(nameof(TeacherFullName));
            }
        }

        public string Classroom
        {
            get => _classroom;
            set
            {
                _classroom = $"Sala: {value}";
                OnPropertyChanged(nameof(Subject));
            }
        }

        public string ClassName
        {
            get => _className;
            set
            {
                _className = $"Klasa: {value}";
                OnPropertyChanged(nameof(ClassName));
            }
        }   
    }
}
