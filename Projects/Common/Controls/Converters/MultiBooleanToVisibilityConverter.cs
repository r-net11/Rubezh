using System;
using System.Windows;
using System.Windows.Data;

namespace Controls.Converters
{
    class MultiBooleanToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values,
                                Type targetType,
                                object parameter,
                                System.Globalization.CultureInfo culture)
        {
            bool visible = true;
            foreach (object value in values)
                if (value is bool)
                    visible = visible && (bool)value;
            if (parameter != null && System.Convert.ToBoolean(parameter))
            {
                return visible ? Visibility.Collapsed : Visibility.Visible;
            }
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value,
                                    Type[] targetTypes,
                                    object parameter,
                                    System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
