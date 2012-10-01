using System;
using System.Windows.Data;
using FiresecAPI;
using SettingsModule.ViewModels;

namespace SettingsModule.Converters
{
    public class ThemeConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is SettingsViewModel.Theme)
            {
                return ((SettingsViewModel.Theme)value).ToDescription();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
