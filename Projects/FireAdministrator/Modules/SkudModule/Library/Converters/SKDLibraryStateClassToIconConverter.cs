using System;
using System.Windows.Data;
using Controls;
using XFiresecAPI;

namespace SKDModule.Converters
{
	public class SKDLibraryStateClassToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var stateClass = (XStateClass)value;
			if (stateClass == XStateClass.No)
				return null;
			return stateClass.ToIconSource();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}