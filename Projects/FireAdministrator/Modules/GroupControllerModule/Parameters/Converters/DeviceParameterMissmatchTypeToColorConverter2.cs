using System;
using System.Windows.Data;
using System.Windows.Media;
using GKModule.ViewModels;

namespace GKModule.Converters
{
	public class DeviceParameterMissmatchTypeToColorConverter2 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			DeviceParameterMissmatchType deviceParameterMissmatchType = (DeviceParameterMissmatchType)value;
			switch (deviceParameterMissmatchType)
			{
				case DeviceParameterMissmatchType.Unknown:
					return Brushes.LightGray;

				case DeviceParameterMissmatchType.Unequal:
					return Brushes.DarkRed;

				case DeviceParameterMissmatchType.Equal:
					return Brushes.White;
			}
			return Brushes.White;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}