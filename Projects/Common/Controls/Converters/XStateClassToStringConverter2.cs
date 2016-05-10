using StrazhAPI;
using StrazhAPI.GK;
using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class XStateClassToStringConverter2 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var stateClass = (XStateClass)value;
			if (stateClass == XStateClass.Fire1)
			{
                return Resources.Language.Converters.XStateClassToStringConverter2.Fire1;
			}
			if (stateClass == XStateClass.Fire2)
			{
                return Resources.Language.Converters.XStateClassToStringConverter2.Fire2;
			}
			var result = stateClass.ToDescription();
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (XStateClass)value;
		}
	}
}