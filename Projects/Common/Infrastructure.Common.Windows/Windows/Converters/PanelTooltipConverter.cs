using System;
using System.Windows.Data;

namespace Infrastructure.Common.Windows.Windows.Converters
{
	public class PanelTooltipConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var isLeftPanel = parameter == null || System.Convert.ToBoolean(parameter);
			var isVisible = System.Convert.ToBoolean(value);
			return isLeftPanel ?
				(isVisible ? "Свернуть левую часть" : "Развернуть планы") :
				(isVisible ? "Свернуть планы" : "Развернуть левую часть");
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}