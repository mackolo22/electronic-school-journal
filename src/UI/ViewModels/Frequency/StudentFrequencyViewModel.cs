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
        private readonly IUsersRepository _usersRepository;
        private DateTime? _selectedDate;

        public StudentFrequencyViewModel(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
            Attendances = new List<Attendance>();
        }

        public long? StudentId { get; set; }
        public Student Student { get; set; }

        public RelayCommand LoadedCommand => new RelayCommand(async (parameter) => await ExecuteLoadedAsync(parameter), () => true);
        private async Task ExecuteLoadedAsync(object parameter)
        {
            var user = await _usersRepository.GetAsync(nameof(Student), StudentId.ToString());
            Student = user as Student;
            if (!String.IsNullOrEmpty(Student.SerializedAttendances))
            {
                Student.Attendances = JsonConvert.DeserializeObject<List<Attendance>>(Student.SerializedAttendances);
            }
            else
            {
                Student.Attendances = new List<Attendance>();
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
                    MessageBoxHelper.ShowErrorMessageBox("Brak sprawdzonych obecności w podanym dniu.");
                }

                OnPropertyChanged(nameof(SelectedDate));
                OnPropertyChanged(nameof(Attendances));
            }
        }

        public List<Attendance> Attendances { get; set; }
    }
}
