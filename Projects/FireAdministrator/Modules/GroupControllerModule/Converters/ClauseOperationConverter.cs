using System;
using System.Windows.Data;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKModule.Converters
{
	public class ClauseOperationConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is ClauseOperationType)
			{
				return ((ClauseOperationType)value).ToDescription();
			}
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}