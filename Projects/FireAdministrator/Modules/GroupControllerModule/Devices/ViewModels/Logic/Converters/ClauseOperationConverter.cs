using System;
using System.Windows.Data;
using FiresecAPI.Models;
using XFiresecAPI;

namespace GKModule.ViewModels.Converters
{
    public class ClauseOperationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ClauseOperationType)
            {
                return ((ClauseOperationType)value).ToDescription();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}