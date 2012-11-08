using System;
using System.Windows.Data;
using FiresecAPI;

namespace Controls.Converters
{
    public class IntToEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
			if (!(value is int) || !targetType.IsEnum)
				throw new NotSupportedException();
			return Enum.ToObject(targetType, (int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToInt32(value);
        }
    }
}