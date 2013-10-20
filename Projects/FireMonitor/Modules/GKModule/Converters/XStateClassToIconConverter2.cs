using System;
using System.Windows.Data;
using System.Windows.Media;
using XFiresecAPI;
using Controls;

namespace GKModule.Converters
{
    public class XStateClassToIconConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var stateClass = (XStateClass)value;
            if (stateClass == XStateClass.Off)
                return "";
            return stateClass.ToIconSource();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}