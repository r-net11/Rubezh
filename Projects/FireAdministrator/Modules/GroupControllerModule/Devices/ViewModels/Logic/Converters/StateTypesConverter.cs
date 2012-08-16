using System;
using System.Windows.Data;
using FiresecAPI;
using XFiresecAPI;

namespace GKModule.ViewModels.Converters
{
	public class StateTypesConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is XStateType)
			{
				return ((XStateType)value).ToDescription();
			}
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}