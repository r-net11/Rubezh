using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace Controls
{
    internal class InstructionTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return EnumsConverter.InstructionTypeToString((InstructionType) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}