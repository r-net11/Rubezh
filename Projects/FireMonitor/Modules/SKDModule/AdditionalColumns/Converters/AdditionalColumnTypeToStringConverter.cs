using System;
using System.Windows.Data;
using RubezhAPI;
using RubezhAPI.SKD;

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