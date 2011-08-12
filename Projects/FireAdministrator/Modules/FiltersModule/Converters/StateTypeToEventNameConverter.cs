using System;
using System.Windows.Data;

namespace FiltersModule.Converters
{
    public class StateTypeToEventNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return FiresecAPI.Models.EnumsConverter.StateTypeToEventName((FiresecAPI.Models.StateType) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}