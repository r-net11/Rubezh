using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using DevicesModule.ViewModels;
using System.Windows.Media;

namespace DevicesModule.Converters
{
	public class DeviceParameterMissmatchTypeToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			DeviceParameterMissmatchType deviceParameterMissmatchType = (DeviceParameterMissmatchType)value;
			switch(deviceParameterMissmatchType)
			{
				case DeviceParameterMissmatchType.Unknown:
					return Brushes.Gray;

				case DeviceParameterMissmatchType.Unequal:
					return Brushes.Red;

				case DeviceParameterMissmatchType.Equal:
					return Brushes.Black;
			}
			return Brushes.Black;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}