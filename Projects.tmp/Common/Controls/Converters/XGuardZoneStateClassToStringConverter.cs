using System;
using System.Windows.Data;
using RubezhAPI;
using RubezhAPI.GK;

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

				case XStateClass.TurningOff:
					return "Снимается с охраны";

				case XStateClass.Fire1:
					return "Тревога";
			}
			return ((XStateClass)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (XStateClass)value;
		}
	}
}