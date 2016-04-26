using FiresecAPI;
using FiresecAPI.GK;
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
					return Resources.Language.XStateClassToSKDStringConverter.On;

				case XStateClass.Off:
					return Resources.Language.XStateClassToSKDStringConverter.Off;

				case XStateClass.TurningOff:
			        return Resources.Language.XStateClassToSKDStringConverter.TurningOff;

				case XStateClass.Fire1:
					return Resources.Language.XStateClassToSKDStringConverter.Fire1;
			}
			return stateClass.ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (XStateClass)value;
		}
	}
}