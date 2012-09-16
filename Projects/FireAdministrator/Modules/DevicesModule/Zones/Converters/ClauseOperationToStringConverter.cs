using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.Models;

namespace DevicesModule.Converters
{
    public class ClauseOperationToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
			if (value is ZoneLogicOperation)
				return ((ZoneLogicOperation)value).ToDescription();
			return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}