using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.GK;

namespace Controls.Converters
{
	public class XAlarmTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((GKAlarmType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}