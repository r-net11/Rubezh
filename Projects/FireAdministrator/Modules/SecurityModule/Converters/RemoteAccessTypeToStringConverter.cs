using System;
using System.Windows.Data;
using StrazhAPI;
using StrazhAPI.Models;

namespace SecurityModule.Converters
{
	public class RemoteAccessTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((RemoteAccessType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}