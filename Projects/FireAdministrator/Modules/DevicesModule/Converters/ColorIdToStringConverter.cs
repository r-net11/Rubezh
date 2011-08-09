using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace DevicesModule.Converters
{
    public class ColorIdToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            IndicatorColorType indicatorColorType = (IndicatorColorType) value;
            switch (indicatorColorType)
            {
                case IndicatorColorType.None:
                    return "Нет";

                case IndicatorColorType.Red:
                    return "Красный";

                case IndicatorColorType.Green:
                    return "Зеленый";

                case IndicatorColorType.Orange:
                    return "Оранжевый";

                case IndicatorColorType.RedBlink:
                    return "Красный мигающий";

                case IndicatorColorType.GreenBlink:
                    return "Зеленый мигающий";

                case IndicatorColorType.OrangeBlink:
                    return "Оранжевый мигающий";

                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}