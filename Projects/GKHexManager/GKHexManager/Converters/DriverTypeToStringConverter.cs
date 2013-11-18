using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using XFiresecAPI;

namespace HexManager.Converters
{
	public class DriverTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			XDriverType driverType = (XDriverType)value;
            switch (driverType)
            {
                case XDriverType.GK:
                    return "ГК";

                case XDriverType.KAU:
                    return "КАУ";

                case XDriverType.RSR2_KAU:
                    return "КАУ RSR2";
            }
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}