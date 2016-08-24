using System;
using System.Windows.Data;
using Localization.SKD.Common;
using StrazhAPI.SKD;

namespace Controls.Converters
{
	public class DeactivationTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch((LogicalDeletationType)value)
			{
				case LogicalDeletationType.Active:
					return CommonResources.OnlyActive;

				case LogicalDeletationType.Deleted:
					return CommonResources.NotActive;

				case LogicalDeletationType.All:
					return CommonResources.ActiveAndNotActive;
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (LogicalDeletationType)value;
		}
	}
}