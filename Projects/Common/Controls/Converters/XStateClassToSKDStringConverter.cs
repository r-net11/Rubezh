using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.GK;

namespace Controls.Converters
{
	public class XStateClassToSKDStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var stateClass = (XStateClass)value;
			switch(stateClass)
			{
				case XStateClass.On:
					return "Открыто";

				case XStateClass.Off:
					return "Закрыто";
			}
			return stateClass.ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (XStateClass)value;
		}
	}
}