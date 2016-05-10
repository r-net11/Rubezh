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
                    return Resources.Language.Converters.DoorStateClassToStringConverter.On;

				case XStateClass.Off:
                    return Resources.Language.Converters.DoorStateClassToStringConverter.Off;

				case XStateClass.TurningOn:
                    return Resources.Language.Converters.DoorStateClassToStringConverter.TurningOn;

				case XStateClass.TurningOff:
                    return Resources.Language.Converters.DoorStateClassToStringConverter.TurningOff;

				case XStateClass.Fire1:
                    return Resources.Language.Converters.DoorStateClassToStringConverter.Fire1;
			}
			return ((XStateClass)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (XStateClass)value;
		}
	}
}