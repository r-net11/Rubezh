using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace DevicesModule.Converters
{
    public class ClauseStateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ZoneLogicState)
                return ((ZoneLogicState) value).ToDescription();
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}