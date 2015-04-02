using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace Controls.Converters
{
	public class BoolToSelectionModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (bool)value ? SelectionMode.Extended : SelectionMode.Single;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}