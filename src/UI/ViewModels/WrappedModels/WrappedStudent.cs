using ApplicationCore.Enums;
using ApplicationCore.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UI.ViewModels.WrappedModels
{
    public class WrappedStudent : BindableObject
    {
        private ObservableCollection<WrappedGrade> _grades;
        private string _average;
        private bool _presence;
        private bool _absence;
        private bool _justifiedAbsence;
        private AttendanceType _attendanceTypeInSelectedDay;

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

        public AttendanceType AttendanceTypeInSelectedDay
        {
            get => _attendanceTypeInSelectedDay;
            set
            {
                if (value == AttendanceType.Presence)
                {
                    Presence = true;
                }
                else if (value == AttendanceType.Absence)
                {
                    Absence = true;
                }
                else if (value == AttendanceType.JustifiedAbsence)
                {
                    JustifiedAbsence = true;
                }
                else if (value == AttendanceType.None)
                {
                    _presence = _absence = _justifiedAbsence = false;
                    OnPropertyChanged(nameof(Presence));
                    OnPropertyChanged(nameof(Absence));
                    OnPropertyChanged(nameof(JustifiedAbsence));
                }

                _attendanceTypeInSelectedDay = value;
                OnPropertyChanged(nameof(AttendanceTypeInSelectedDay));
            }
        }

        public bool Presence
        {
            get => _presence;
            set
            {
                if (value == false)
                {
                    return;
                }

                _attendanceTypeInSelectedDay = AttendanceType.Presence;
                _presence = value;
                _absence = false;
                _justifiedAbsence = false;
                OnPropertyChanged(nameof(Presence));
                OnPropertyChanged(nameof(Absence));
                OnPropertyChanged(nameof(JustifiedAbsence));
                OnPropertyChanged(nameof(AttendanceTypeInSelectedDay));
            }
        }

        public bool Absence
        {
            get => _absence;
            set
            {
                if (value == false)
                {
                    return;
                }

                _attendanceTypeInSelectedDay = AttendanceType.Absence;
                _absence = value;
                _presence = false;
                _justifiedAbsence = false;
                OnPropertyChanged(nameof(Presence));
                OnPropertyChanged(nameof(Absence));
                OnPropertyChanged(nameof(JustifiedAbsence));
                OnPropertyChanged(nameof(AttendanceTypeInSelectedDay));
            }
        }

        public bool JustifiedAbsence
        {
            get => _justifiedAbsence;
            set
            {
                if (value == false)
                {
                    return;
                }

                _attendanceTypeInSelectedDay = AttendanceType.JustifiedAbsence;
                _justifiedAbsence = value;
                _presence = false;
                _absence = false;
                OnPropertyChanged(nameof(Presence));
                OnPropertyChanged(nameof(Absence));
                OnPropertyChanged(nameof(JustifiedAbsence));
                OnPropertyChanged(nameof(AttendanceTypeInSelectedDay));
            }
        }
    }
}
