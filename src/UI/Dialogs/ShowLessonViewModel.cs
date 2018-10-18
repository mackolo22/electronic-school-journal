using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System.Threading.Tasks;
using UI.ViewModels;

namespace UI.Dialogs
{
    public class ShowLessonViewModel : ViewModelBase
    {
        private readonly ITableStorageRepository _repository;

        public ShowLessonViewModel(ITableStorageRepository repository)
        {
            _repository = repository;
        }

        public string Subject { get; set; }
        public string Term { get; set; }
        public long? TeacherId { get; set; }
        public string TeacherFullName { get; set; }
        public string Classroom { get; set; }
        public string ClassName { get; set; }

        public RelayCommand LoadedCommand => new RelayCommand(async (parameter) => await ExecuteLoadedAsync(parameter), () => true);
        private async Task ExecuteLoadedAsync(object parameter)
        {
            if (TeacherId != null)
            {
                var teacher = await _repository.GetAsync<Teacher>(nameof(Teacher), TeacherId.ToString());
                if (teacher != null)
                {
                    TeacherFullName = teacher.FullName;
                    OnPropertyChanged(nameof(TeacherFullName));
                }
            }


        }
    }
}
