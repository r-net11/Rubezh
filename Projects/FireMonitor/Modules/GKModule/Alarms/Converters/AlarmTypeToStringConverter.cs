using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace GKModule.Converters
{
    public class AlarmTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((XAlarmType) value)
            {
				case XAlarmType.NPT:
					return "НПТ";

				case XAlarmType.Fire1:
                    return "Пожар 1";

				case XAlarmType.Fire2:
					return "Пожар 2";

				case XAlarmType.Attention:
                    return "Внимание";

				case XAlarmType.Failure:
                    return "Неисправность";

				case XAlarmType.Ignore:
                    return "Отключение";

				case XAlarmType.Info:
                    return "Информация";

				case XAlarmType.Service:
                    return "Обслуживание";

				case XAlarmType.Auto:
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