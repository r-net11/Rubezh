using System;
using System.Globalization;
using System.Windows.Data;
using Localization.StrazhService.Monitor.Common;

namespace StrazhService.Monitor.Converters
{
	public class ServiceStateToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var serviceState = (ServiceState) value;
			var result = string.Empty;
			switch (serviceState)
			{
				case ServiceState.Stoped:
					result = CommonResources.ServerStoped;
					break;
				case ServiceState.Starting:
					result = CommonResources.ServerStarts;
					break;
				case ServiceState.Started:
					result = CommonResources.ServerStarted;
					break;
			}
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}