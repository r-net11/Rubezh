using StrazhAPI.GK;
using System;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Controls.Converters
{
	public class XStateClassToIconConverter2 : IMultiValueConverter, IValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			try
			{
				var stateClass = (XStateClass)values[0];
				if (stateClass == XStateClass.Norm || stateClass == XStateClass.Off)
					return null;
				var isAm = values.Count() > 1 && values[1] is bool && (bool)values[1];
				var uri = new Uri("pack://application:,,," + stateClass.ToIconSource(isAm));
				var bitMap = new BitmapImage(uri);
				return bitMap;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var stateClass = (XStateClass)value;
			return stateClass.ToIconSource();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}