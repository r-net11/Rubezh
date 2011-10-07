using System.Windows;
using System.Windows.Data;

namespace Controls
{
    public class HighlightCornerRadiusConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var corners = (CornerRadius) value;
            if (corners != null)
            {
                corners.BottomLeft = 0;
                corners.BottomRight = 0;

                return corners;
            }
            return null;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}