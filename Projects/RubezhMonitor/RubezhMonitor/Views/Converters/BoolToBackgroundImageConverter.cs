using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace RubezhMonitor.Views.Converters
{
	internal class BoolToBackgroundImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((bool) value) ?
				new Image() { Source = new BitmapImage(new Uri("Images/sound.png", UriKind.Relative)) } :
				new Image() { Source = new BitmapImage(new Uri("Images/mute.png", UriKind.Relative)) };
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}