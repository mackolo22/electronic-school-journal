using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Helpers;

namespace UI.ViewModels
{
    public class StudentFrequencyViewModel : BaseViewModel
    {
        private readonly ITableStorageRepository _repository;
        private DateTime? _selectedDate;

        public StudentFrequencyViewModel(ITableStorageRepository repository)
        {
            _repository = repository;
            Attendances = new List<Attendance>();
        }

        public long? StudentId { get; set; }
        public Student Student { get; set; }

        public RelayCommand LoadedCommand => new RelayCommand(async (parameter) => await ExecuteLoadedAsync(parameter), () => true);
        private async Task ExecuteLoadedAsync(object parameter)
        {
            Student = await _repository.GetAsync<Student>(nameof(Student), StudentId.ToString());
            if (!String.IsNullOrEmpty(Student.SerializedAttendances))
            {
                Student.Attendances = JsonConvert.DeserializeObject<List<Attendance>>(Student.SerializedAttendances);
            }
        }

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                Attendances = Student.Attendances
                    .Where(x => x.Date == value)
                    .OrderBy(x => x.Subject)
                    .OrderBy(x => x.LessonTerm.LessonNumber)
                    .ToList();
                _selectedDate = value;

                if (Attendances.Count == 0)
                {
                    MessageBoxHelper.ShowErrorMessageBox("Brak sprawdzonych obecności w podanym dniu.", "Uwaga");
                }

                OnPropertyChanged(nameof(SelectedDate));
                OnPropertyChanged(nameof(Attendances));
            }
        }

        public List<Attendance> Attendances { get; set; }
    }
}
