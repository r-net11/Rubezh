using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace JournalModule.Converters
{
    public class SubsystemTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return EnumHelper.ToString((SubsystemType) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}