using Localization.Common.Controls;
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
					return CommonResources.On;

				case XStateClass.Off:
					return CommonResources.Off;

				case XStateClass.TurningOff:
					return CommonResources.TurningOff;

				case XStateClass.Fire1:
					return CommonResources.Alarm;
			}
			return stateClass.ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (XStateClass)value;
		}
	}
}