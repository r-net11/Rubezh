using System;
using System.Globalization;
using System.Windows.Data;

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
					result = "Сервер остановлен";
					break;
				case ServiceState.Starting:
					result = "Сервер запускается";
					break;
				case ServiceState.Started:
					result = "Сервер запущен";
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