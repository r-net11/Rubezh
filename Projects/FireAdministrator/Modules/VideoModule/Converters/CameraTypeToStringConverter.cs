using System;
using System.Windows.Data;
using FiresecAPI;

namespace VideoModule.Converters
{
	class CameraTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((CameraType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (CameraType)value;
		}
	}
}
