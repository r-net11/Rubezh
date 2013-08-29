using System;
using System.Windows.Data;
using XFiresecAPI;

namespace Controls.Converters
{
	public class XStateClassToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var stateClass = (XStateClass)value;
			if (stateClass == XStateClass.Norm)
				return null;
			if (stateClass == XStateClass.Off)
				return null;

			return "/Controls;component/StateClassIcons/" + stateClass.ToString() + ".png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}