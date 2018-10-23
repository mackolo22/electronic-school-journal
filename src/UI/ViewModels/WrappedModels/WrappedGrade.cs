using System;

namespace UI.ViewModels.WrappedModels
{
    public class WrappedGrade : BindableObject
    {
        private double _value;
        private string _comment;

        public int Id { get; set; }
        public long? StudentId { get; set; }

        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }

        }

        public string Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                OnPropertyChanged(nameof(Comment));
            }
        }

        public DateTime LastModificationDate { get; set; }
    }
}
