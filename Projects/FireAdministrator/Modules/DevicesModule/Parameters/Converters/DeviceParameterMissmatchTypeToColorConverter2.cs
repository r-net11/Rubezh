using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using DevicesModule.ViewModels;
using System.Windows.Media;

namespace DevicesModule.Converters
{
	public class DeviceParameterMissmatchTypeToColorConverter2 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			DeviceParameterMissmatchType deviceParameterMissmatchType = (DeviceParameterMissmatchType)value;
			switch(deviceParameterMissmatchType)
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