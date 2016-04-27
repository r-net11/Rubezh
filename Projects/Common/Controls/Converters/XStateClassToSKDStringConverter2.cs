using FiresecAPI;
using FiresecAPI.GK;
using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class XStateClassToSKDStringConverter2 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var stateClass = (XStateClass)value;
			switch (stateClass)
			{
				case XStateClass.On:
                    return Resources.Language.Converters.XStateClassToSKDStringConverter2.On;
				case XStateClass.Off:
                    return Resources.Language.Converters.XStateClassToSKDStringConverter2.Off;
				case XStateClass.TurningOff:
                    return Resources.Language.Converters.XStateClassToSKDStringConverter2.TurningOff;
				case XStateClass.Fire1:
                    return Resources.Language.Converters.XStateClassToSKDStringConverter2.Fire1;
			}
			return stateClass.ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (XStateClass)value;
		}
	}
}