using System;
using System.Windows.Data;
using XFiresecAPI;

namespace Controls.Converters
{
    public class XStateTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var stateType = (XStateType)value;
            if (stateType == XStateType.Norm)
                return null;

            return "/Controls;component/GKIcons/" + stateType.ToString() + ".png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
