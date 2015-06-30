using System;
using System.Windows.Data;
using System.Windows.Media;
using GKModule.ViewModels;

namespace GKModule.Converters
{
	public class DeviceParameterMissmatchTypeToColorConverter3 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			DeviceParameterMissmatchType deviceParameterMissmatchType = (DeviceParameterMissmatchType)value;
			switch (deviceParameterMissmatchType)
			{
				case DeviceParameterMissmatchType.Unknown:
					return Brushes.Black;
				case DeviceParameterMissmatchType.Equal:
					return Brushes.Black;
				case DeviceParameterMissmatchType.Unequal:
					return Brushes.Red;
			}
			return Brushes.White;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}