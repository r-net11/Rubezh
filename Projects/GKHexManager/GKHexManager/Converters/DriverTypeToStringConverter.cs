using System;
using System.Windows.Data;
using RubezhAPI.GK;

namespace HexManager.Converters
{
	public class DriverTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value.ToString() == "")
				return "";
			GKDriverType driverType = (GKDriverType)value;
            switch (driverType)
            {
				case GKDriverType.GK:
                    return "ГК";

				case GKDriverType.RSR2_KAU:
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