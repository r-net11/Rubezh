using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FireMonitor.Views.Converters
{
    class BoolToBackgroundImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("Images/sound.png", UriKind.Relative));
                return image;
            }
            else
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("Images/mute.png", UriKind.Relative));
                return image;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
