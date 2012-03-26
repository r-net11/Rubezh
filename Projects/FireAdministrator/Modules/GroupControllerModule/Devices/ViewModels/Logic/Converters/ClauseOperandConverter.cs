using System;
using System.Windows.Data;
using GroupControllerModule.Models;
using FiresecAPI.Models;

namespace GroupControllerModule.ViewModels.Converters
{
    public class ClauseOperandConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ClauseOperandType)
            {
                return ((ClauseOperandType)value).ToDescription();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}