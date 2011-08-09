using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Controls
{
    public class ColorToAlphaColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush brush = (SolidColorBrush) value;

            if (brush != null)
            {
                Color color = brush.Color;
                color.A = byte.Parse(parameter.ToString());

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