using System;
using System.Windows.Data;
using FiresecAPI.Models;
using Common;

namespace Controls.Converters
{
    internal class InstructionTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return EnumHelper.ToString((InstructionType)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}