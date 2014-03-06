using System;
using System.Windows.Data;
using FiresecAPI;
using XFiresecAPI;

namespace Controls.Converters
{
	internal class XInstructionTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((XInstructionType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}