using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace Controls.Converters
{
	internal class ReportTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var str = value as string;
			if (str != null && str == "")
				return null;
			return ((ReportType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}