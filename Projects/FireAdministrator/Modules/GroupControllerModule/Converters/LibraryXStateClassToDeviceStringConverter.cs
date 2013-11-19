using System;
using System.Windows.Data;
using FiresecAPI;
using XFiresecAPI;

namespace GKModule.Converters
{
	public class LibraryXStateClassToDeviceStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			XStateClass stateClass = (XStateClass)value;
			var result = stateClass.ToDescription();
			if (stateClass == XStateClass.Fire1)
				return "Сработка 1";
			if (stateClass == XStateClass.Fire2)
				return "Сработка 2";
			if (stateClass == XStateClass.No)
				return "Базовый рисунок";
			XDevice device = parameter as XDevice;
			if (device != null)
			{
				if (device.DriverType == XDriverType.Valve)
				{
					switch (stateClass)
					{
						case XStateClass.Off:
							result = "Закрыто";
							break;

						case XStateClass.On:
							result = "Открыто";
							break;

						case XStateClass.TurningOff:
							result = "Закрывается";
							break;

						case XStateClass.TurningOn:
							result = "Открывается";
							break;
					}
				}
			}
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (XStateClass)value;
		}
	}
}