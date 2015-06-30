using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.Automation;

namespace Controls.Converters
{
	public class ObjectTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is ObjectType))
				return "";
			return ((ObjectType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (ObjectType)value;
		}
	}
}