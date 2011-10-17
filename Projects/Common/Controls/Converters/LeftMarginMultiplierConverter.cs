using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;
using System.Globalization;
using System.Windows;

namespace Controls.Converters
{
    public class LeftMarginMultiplierConverter : IValueConverter
    {
        public double Length { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as TreeViewItem;
            if (item == null)
                return new Thickness(0);

            return new Thickness(Length * item.GetDepth(), 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }

    public static class TreeViewItemExtensions
    {
        public static int GetDepth(this TreeViewItem item)
        {
            FrameworkElement elem = item;
            while (elem.Parent != null)
            {
                var tvi = elem.Parent as TreeViewItem;
                if (tvi != null)
                    return tvi.GetDepth() + 1;
                elem = elem.Parent as FrameworkElement;
            }
            return 0;
        }
    }

}
