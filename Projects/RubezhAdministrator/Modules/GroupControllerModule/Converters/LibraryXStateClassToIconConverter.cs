using System;
using System.Windows.Data;
using Controls;
using RubezhAPI.GK;

namespace GKModule.Converters
{
	public class LibraryXStateClassToIconConverter : IValueConverter
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