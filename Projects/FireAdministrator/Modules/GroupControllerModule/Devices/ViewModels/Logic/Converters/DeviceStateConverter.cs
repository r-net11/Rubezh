using System;
using System.Windows.Data;
using FiresecAPI.Models;
using GroupControllerModule.Models;

namespace GroupControllerModule.ViewModels.Converters
{
    public class DeviceStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is XStateType)
            {
                return ((XStateType)value).ToDescription();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}