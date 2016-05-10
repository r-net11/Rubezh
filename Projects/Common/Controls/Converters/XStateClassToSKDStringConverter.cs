using StrazhAPI;
using StrazhAPI.GK;
using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class XStateClassToSKDStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var stateClass = (XStateClass)value;
			switch (stateClass)
			{
				case XStateClass.On:
                    return Resources.Language.Converters.XStateClassToSKDStringConverter.On;

				case XStateClass.Off:
                    return Resources.Language.Converters.XStateClassToSKDStringConverter.Off;

				case XStateClass.TurningOff:
                    return Resources.Language.Converters.XStateClassToSKDStringConverter.TurningOff;

				case XStateClass.Fire1:
                    return Resources.Language.Converters.XStateClassToSKDStringConverter.Fire1;
			}
			return stateClass.ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (XStateClass)value;
		}
	}
}