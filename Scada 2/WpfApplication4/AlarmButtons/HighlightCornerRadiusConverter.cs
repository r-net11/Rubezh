using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace AlarmButtons
{
    public class HighlightCornerRadiusConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CornerRadius corners = (CornerRadius)value;

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
