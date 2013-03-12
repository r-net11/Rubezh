using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.Models;

namespace GKModule.Converters
{
	public class MROMessageNoToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is ZoneLogicMROMessageNo)
				return ((ZoneLogicMROMessageNo)value).ToDescription();
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}