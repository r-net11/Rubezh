using System;
using System.Windows.Data;

namespace Infrastructure.Common.Converters
{
    public class IdToDeviceCategoryNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((int) value)
            {
                case 0:
                    return "Прочие устройства";

                case 1:
                    return "Прибор";

                case 2:
                    return "Датчик";

                case 3:
                    return "Исполнительное устройство";

                case 4:
                    return "Сеть передачи данных";

                case 5:
                    return "Удаленный сервер";

                case 6:
                    return "[Без устройства]";

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