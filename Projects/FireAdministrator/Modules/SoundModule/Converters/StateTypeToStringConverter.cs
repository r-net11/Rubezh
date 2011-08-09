using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using FiresecAPI.Models;

namespace SoundsModule.Converters
{
    class StateTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            StateType Value = (StateType)value;
            switch (Value)
            {
                case StateType.Fire:
                    return "Тревога";

                case StateType.Attention:
                    return "Внимание (предтревожное)";

                case StateType.Failure:
                    return "Неисправность";

                case StateType.Service:
                    return "Требуется обслуживание";

                case StateType.Off:
                    return "Обход устройств";

                case StateType.Unknown:
                    return "Неопределено";

                case StateType.Info:
                    return "Норма(*)";

                case StateType.Norm:
                    return "Норма";

                case StateType.No:
                    return "Нет состояния";

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
