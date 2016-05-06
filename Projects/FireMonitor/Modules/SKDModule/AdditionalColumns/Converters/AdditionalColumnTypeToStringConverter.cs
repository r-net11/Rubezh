using System;
using System.Windows.Data;
using StrazhAPI;
using StrazhAPI.SKD;

namespace SKDModule.Converters
{
	public class DataTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is AdditionalColumnDataType)
				return ((AdditionalColumnDataType)value).ToDescription();
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}