using System;
using System.Globalization;
using System.Windows.Data;
using static UI.ViewModels.TimeTableViewModel;

namespace UI.Converters
{
    public class LessonButtonIsEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var wrappedLesson = value as WrappedLesson;
            if (!String.IsNullOrWhiteSpace(wrappedLesson.ClassName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
