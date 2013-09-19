using System;
using System.Windows.Data;
using XFiresecAPI;

namespace Controls.Converters
{
	public class XStateTypeToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is XStateBit))
				return null;

			var stateType = (XStateBit)value;

			if (stateType == XStateBit.Norm)
				return null;

			return "/Controls;component/StateClassIcons/" + stateType.ToString() + ".png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}