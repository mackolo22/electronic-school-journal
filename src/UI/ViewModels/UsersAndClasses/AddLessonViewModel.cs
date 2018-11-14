using ApplicationCore.Enums;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UI.Helpers;

namespace UI.ViewModels
{
    public class AddLessonViewModel : BaseViewModel
    {
        private Window _window;

        public bool ChangesSaved { get; set; }
        public List<Teacher> AllTeachers { get; set; }
        public Teacher SelectedTeacher { get; set; }
        public IEnumerable<Subject> Subjects { get => Enum.GetValues(typeof(Subject)).Cast<Subject>(); }
        public Subject SelectedSubject { get; set; }
        public string Classroom { get; set; }

        public RelayCommand LoadedCommand => new RelayCommand(ExecuteLoaded, () => true);
        private void ExecuteLoaded(object parameter)
        {
            _window = parameter as Window;
        }

        public RelayCommand CloseWindowCommand => new RelayCommand(ExecuteCloseWindow, () => true);
        private void ExecuteCloseWindow(object parameter)
        {
            bool saveChanges = (bool)parameter;
            if (saveChanges)
            {
                if (SelectedSubject != 0 && SelectedTeacher != null && !String.IsNullOrWhiteSpace(Classroom))
                {
                    ChangesSaved = true;
                    _window.Close();
                }
                else
                {
                    ChangesSaved = false;
                    MessageBoxHelper.ShowErrorMessageBox("Wypełnij wszystkie pola.");
                }
            }
            else
            {
                ChangesSaved = false;
                _window.Close();
            }
        }
    }
}
