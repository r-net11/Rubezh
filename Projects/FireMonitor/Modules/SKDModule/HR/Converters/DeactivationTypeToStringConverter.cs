using System;
using System.Windows.Data;
using RubezhAPI.SKD;

namespace Controls.Converters
{
	public class DeactivationTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch((LogicalDeletationType)value)
			{
				case LogicalDeletationType.Active:
					return "Только активные";

				case LogicalDeletationType.Deleted:
					return "Только не активные";

				case LogicalDeletationType.All:
					return "Активные и не активные";
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (LogicalDeletationType)value;
		}
	}
}