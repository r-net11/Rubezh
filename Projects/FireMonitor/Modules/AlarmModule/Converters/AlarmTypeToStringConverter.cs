using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace AlarmModule.Converters
{
    public class AlarmTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            AlarmType alarmType = (AlarmType)value;
            switch (alarmType)
            {
                case AlarmType.Fire:
                    return "Пожар";

                case AlarmType.Attention:
                    return "Внимание";

                case AlarmType.Failure:
                    return "Неисправность";

                case AlarmType.Off:
                    return "Отключение";

                case AlarmType.Info:
                    return "Информация";

                case AlarmType.Service:
                    return "Обслуживание";

                case AlarmType.Auto:
                    return "Автоматика";

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
