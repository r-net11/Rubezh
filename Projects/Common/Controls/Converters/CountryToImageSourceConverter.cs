using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Common;

namespace Controls.Converters
{
	public class CountryToImageSourceConverter : IValueConverter
	{
		private const string ConnectionStateToImageSource = "pack://application:,,,/Controls;component/Images/CountryFlags/{0}.png";

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				if (value == null || string.IsNullOrEmpty(value.ToString()))
					return new BitmapImage();
				return new BitmapImage(new Uri(string.Format(ConnectionStateToImageSource, value)));
			}
			catch (Exception e)
			{
				Logger.Error(e);
				return new BitmapImage();
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}