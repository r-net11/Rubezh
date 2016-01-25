using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Controls.Converters
{
	public class ConnectionStateToImageSourceConverter : IValueConverter
	{
		private const string ConnectionStateToImageSource = "pack://application:,,,/Controls;component/Images/Video{0}.png";

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return new BitmapImage();
			return new BitmapImage(new Uri(string.Format(ConnectionStateToImageSource, value)));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}