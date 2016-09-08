using System;
using System.Windows.Data;
using Localization.Common.Controls;

namespace Controls.Converters
{
	public class BoolToMinimizeTooltipConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var val = parameter != null && System.Convert.ToBoolean(parameter) ? !(bool)value : (bool)value;
			return val ? CommonResources.Minimize : CommonResources.Maximize;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}