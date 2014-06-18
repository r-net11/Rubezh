using System.Windows.Data;
using FiresecAPI;
using System;
using FiresecAPI.Automation;

namespace Controls.Converters
{
	public class ObjectTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((ObjectType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (ObjectType)value;
		}
	}
}