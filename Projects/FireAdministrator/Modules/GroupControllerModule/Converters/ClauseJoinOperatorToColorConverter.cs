using System;
using System.Windows.Data;
using System.Windows.Media;
using RubezhAPI.GK;

namespace GKModule.Converters
{
	public class ClauseJoinOperatorToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((ClauseJounOperationType)value)
			{
				case ClauseJounOperationType.Or:
					return Brushes.LightGreen;

				case ClauseJounOperationType.And:
					return Brushes.LightSkyBlue;

				default:
					return Brushes.Gray;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}