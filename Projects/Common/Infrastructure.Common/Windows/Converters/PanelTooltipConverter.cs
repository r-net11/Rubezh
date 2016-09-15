using System;
using System.Windows.Data;
using Localization.Common.InfrastructureCommon;

namespace Infrastructure.Common.Windows.Converters
{
	public class PanelTooltipConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var isLeftPanel = parameter == null || System.Convert.ToBoolean(parameter);
			var isVisible = System.Convert.ToBoolean(value);
			return isLeftPanel ?
				(isVisible ? CommonResources.MinimizeLeftPart : CommonResources.MaximizePlans) :
				(isVisible ? CommonResources.MinimizePlans : CommonResources.MaximizeLeftPart);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}