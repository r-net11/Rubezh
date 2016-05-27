using System;
using System.Globalization;
using System.Windows.Data;

namespace StrazhService.Monitor.Converters
{
	public class ServiceStateToColorNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var colorName = string.Empty;
			var serviceState = (ServiceState) value;
			switch (serviceState)
			{
				case ServiceState.Stoped:
					colorName = "Red";
					break;
				case ServiceState.Starting:
					colorName = "Yellow";
					break;
				case ServiceState.Started:
					colorName = "Green";
					break;
			}
			return colorName;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}