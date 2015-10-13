using System;
using System.Windows.Data;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKModule.Converters
{
	public class ClauseConditionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is ClauseConditionType)
			{
				return ((ClauseConditionType)value).ToDescription();
			}
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}