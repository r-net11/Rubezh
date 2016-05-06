using StrazhAPI;
using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class EnumToDescriptionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null || !value.GetType().IsEnum)
				return string.Empty;
			return ((Enum)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}