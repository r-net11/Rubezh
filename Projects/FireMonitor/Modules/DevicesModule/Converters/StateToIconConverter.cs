using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace DevicesModule.Converters
{
	public class StateToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var stateType = (StateType)value;
			if (stateType == StateType.Norm)
				return null;
			return FiresecClient.FileHelper.GetStateIcon((StateType)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}