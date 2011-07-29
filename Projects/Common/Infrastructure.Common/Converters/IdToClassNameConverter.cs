using System;
using System.Windows.Data;

namespace Infrastructure.Common.Converters
{
    public class IdToClassNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((int) value)
            {
                case 0:
                    return "Тревога";

                case 1:
                    return "Внимание";

                case 2:
                    return "Неисправность";

                case 3:
                    return "Требуется обслуживание";

                case 4:
                    return "Тревоги отключены";

                case 6:
                    return "Информация";

                case 7:
                    return "Прочие";

                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
