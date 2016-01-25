using System;
using System.Windows.Data;
using RubezhAPI;
using RubezhAPI.GK;

namespace Controls.Converters
{
	public class MPTDeviceTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((GKMPTDeviceType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (GKMPTDeviceType)value;
		}
	}
}