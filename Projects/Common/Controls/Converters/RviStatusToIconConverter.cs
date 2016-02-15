using RubezhAPI.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Controls.Converters
{
	public class RviStatusToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var rviStatus = (RviStatus)value;
			return "/Controls;component/RviStatusIcons/" + rviStatus + ".png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}