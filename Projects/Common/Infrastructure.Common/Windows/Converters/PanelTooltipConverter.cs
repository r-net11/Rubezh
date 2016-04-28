using System;
using System.Windows.Data;

namespace Infrastructure.Common.Windows.Converters
{
	public class PanelTooltipConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var isLeftPanel = parameter == null || System.Convert.ToBoolean(parameter);
			var isVisible = System.Convert.ToBoolean(value);
			return isLeftPanel ?
                (isVisible ? Resources.Language.Windows.Converters.PanelTooltipConverter.MinimizeLeftPart : Resources.Language.Windows.Converters.PanelTooltipConverter.MaximizePlans) :
                (isVisible ? Resources.Language.Windows.Converters.PanelTooltipConverter.MinimizePlans : Resources.Language.Windows.Converters.PanelTooltipConverter.MaximizeLeftPart);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}