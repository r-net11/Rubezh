using System;
using System.Windows.Data;
using Localization.Common.Controls;

namespace Controls.Converters
{
	public class IsOffToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (bool)value ? CommonResources.PullOff : CommonResources.SwitchOff;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}