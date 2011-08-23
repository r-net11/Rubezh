using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace DevicesModule.Converters
{
    public class ClauseStateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ZoneLogicState)
            {
                ZoneLogicState state = (ZoneLogicState)value;
                switch (state)
                {
                    case ZoneLogicState.AutomaticOn:
                        return "Включение автоматики";

                    case ZoneLogicState.Alarm:
                        return "Тревога";

                    case ZoneLogicState.Fre:
                        return "Пожар";

                    case ZoneLogicState.Warning:
                        return "Внимание";

                    case ZoneLogicState.MPTOn:
                        return "Включение модуля пожаротушения";
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}