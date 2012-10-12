using System;
using System.Windows.Data;
using FiresecAPI;
using Infrastructure.Common.Module;

namespace SettingsModule.Converters
{
    public class ModuleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Module)
            {
                return ((Module)value).ToDescription();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
