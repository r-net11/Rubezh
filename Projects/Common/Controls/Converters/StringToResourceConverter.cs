using System;
using System.Globalization;
using System.Windows.Data;

namespace Controls.Converters
{
	public class StringToResourceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ConverterHelper.GetResource(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}