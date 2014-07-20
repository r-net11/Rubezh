using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.GK;

namespace Controls.Converters
{
	public class XGuardZoneStateClassToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch((XStateClass)value)
			{
				case XStateClass.On:
					return "На Охране";

				case XStateClass.Off:
					return "Снята с охраны";

				case XStateClass.TurningOn:
					return "Ставится на охрану";
			}
			return ((XStateClass)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (XStateClass)value;
		}
	}
}