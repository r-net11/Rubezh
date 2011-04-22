using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace DevicesModule.Converters
{
    public class StateToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string state = (string)value;
            string icon = StateToIcon(state);
            if (icon != null)
            {
                return "../Icons/" + icon + ".ico";
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        string StateToIcon(string state)
        {
            switch (state)
            {
                case "Тревога":
                    return "DS_Critical";

                case "Внимание (предтревожное)":
                    return "DS_Warning";

                case "Неисправность":
                    return "DS_Error";

                case "Требуется обслуживание":
                    return "DS_ServiceRequired";

                case "Обход устройств":
                    return "DS_Mute";

                case "Неопределено":
                    return "DS_Unknown";

                case "Норма(*)":
                    return "DS_Normal";

                case "Норма":
                    return null;

                default:
                    return null;
            }
        }
    }
}
