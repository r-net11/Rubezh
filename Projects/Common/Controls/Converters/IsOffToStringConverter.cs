using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class IsOffToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (bool)value ? Resources.Language.IsOffToStringConverter.PullOff : Resources.Language.IsOffToStringConverter.SwitchOff;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}