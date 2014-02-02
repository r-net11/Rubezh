using System;
using System.Windows.Data;
using System.Windows.Media;

namespace ConfigurationViewer.Converters
{
	public class IsBoldToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var boolValue = (bool)value;
			if (boolValue)
				return Brushes.LightGray;
			return Brushes.White;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}