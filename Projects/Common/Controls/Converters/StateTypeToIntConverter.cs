using System;
using System.Windows.Data;

namespace Controls.Converters
{
    public class StateTypeToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)((FiresecAPI.Models.StateType) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
			throw new NotImplementedException();
        }
    }
}