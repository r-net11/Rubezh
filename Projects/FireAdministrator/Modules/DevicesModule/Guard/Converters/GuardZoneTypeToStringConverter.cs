using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace DevicesModule.Converters
{
	public class GuardZoneTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((GuardZoneType)value)
			{
				case GuardZoneType.Ordinary:
					return "Обычная";

				case GuardZoneType.Delay:
					return "С задержкой входа/выхода";

				case GuardZoneType.CanNotReset:
					return "Без права снятия";

				default:
					return "";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}