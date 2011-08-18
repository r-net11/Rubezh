using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Controls
{
    public class ColorToAlphaColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var brush = value as SolidColorBrush;
            if (brush != null)
            {
                Color color = brush.Color;
                byte tmpColor;
                if (byte.TryParse(parameter as string, out tmpColor))
                {
                    color.A = tmpColor;
                }

                return color;
            }

            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}