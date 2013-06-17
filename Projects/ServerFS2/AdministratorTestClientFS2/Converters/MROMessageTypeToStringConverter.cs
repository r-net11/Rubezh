using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.Models;

namespace AdministratorTestClientFS2.Converters
{
	public class MROMessageTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is ZoneLogicMROMessageType)
				return ((ZoneLogicMROMessageType)value).ToDescription();
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}