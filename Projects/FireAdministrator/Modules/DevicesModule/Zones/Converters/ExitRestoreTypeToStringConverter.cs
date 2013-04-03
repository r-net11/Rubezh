using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.Models;

namespace DevicesModule.Converters
{
	public class ExitRestoreTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
			if (value is ExitRestoreType)
				return ((ExitRestoreType)value).ToDescription();
			return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}