using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Controls.Converters
{
    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string imageSourceString = (string)value;
            Image image = new Image();
            image.VerticalAlignment = VerticalAlignment.Center;
            BitmapImage sourceImage = new BitmapImage();
            sourceImage.BeginInit();
            sourceImage.UriSource = new Uri(imageSourceString);
            sourceImage.EndInit();
            image.Source = sourceImage;
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
