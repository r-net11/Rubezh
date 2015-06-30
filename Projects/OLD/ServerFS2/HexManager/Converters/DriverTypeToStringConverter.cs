using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using FiresecAPI.Models;

namespace HexManager.Converters
{
	public class DriverTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			DriverType driverType = (DriverType)value;
			return DriversHelper.GetDriverNameByDriverType(driverType);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}