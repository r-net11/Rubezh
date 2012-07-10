using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace GKModule.Converters
{
    public class StateToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string icon = EnumsConverter.StateToIcon((StateType) value);
            if (icon != null)
                return FiresecClient.FileHelper.GetIconFilePath(icon + ".ico");
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}