using System;
using System.Windows.Data;

namespace SKDModule.Converters
{
	public class RadioButtonCheckedConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			var boolParameter = (int)parameter == 1;
			return value.Equals(boolParameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			var boolParameter = (int)parameter == 1;
			return value.Equals(true) ? boolParameter : Binding.DoNothing;
		}
	}
}