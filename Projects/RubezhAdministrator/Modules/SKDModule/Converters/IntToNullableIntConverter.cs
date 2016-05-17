using System;
using System.Windows.Data;

namespace SKDModule.Converters
{
	public class IntToNullableIntConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value ?? 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var intVal = value as Nullable<int>;
			return intVal == null || !intVal.HasValue || intVal.Value != 0 ? value : null;
		}
	}
}