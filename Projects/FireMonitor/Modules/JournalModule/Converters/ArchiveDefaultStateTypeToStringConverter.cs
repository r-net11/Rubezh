using System.Windows.Data;
using FiresecAPI.Models;

namespace JournalModule.Converters
{
    public class ArchiveDefaultStateTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return EnumHelper.ToString((ArchiveDefaultStateType) value);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}