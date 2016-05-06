using StrazhAPI;
using StrazhAPI.GK;
using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class DoorStateClassToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((XStateClass)value)
			{
				case XStateClass.On:
					return "Открыта";

				case XStateClass.Off:
					return "Закрыта";

				case XStateClass.TurningOn:
					return "Открывается";

				case XStateClass.TurningOff:
					return "Закрывается";

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