using System;
using System.Windows.Data;
using FiresecAPI;

namespace Controls.Converters
{
	public class EnumToDescriptionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!value.GetType().IsEnum)
				return string.Empty;
			return ((Enum)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}