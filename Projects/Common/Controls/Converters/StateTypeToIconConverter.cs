using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace Controls.Converters
{
    public class StateTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var stateType = (StateType)value;
            if (stateType == StateType.Norm)
                return null;

            return "/Controls;component/StateIcons/" + stateType.ToString() + ".png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}