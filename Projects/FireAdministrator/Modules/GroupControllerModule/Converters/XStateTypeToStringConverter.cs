using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.GK;

namespace GKModule.Converters
{
	public class XStateTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is XStateBit)
			{
				return ((XStateBit)value).ToDescription();
			}
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}