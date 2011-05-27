using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace DevicesModule.Converters
{
    public class ColorIdToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string colorId = (string)value;
            switch (colorId)
            {
                case "0":
                    return "Нет";

                case "1":
                    return "Красный";

                case "2":
                    return "Зеленый";

                case "3":
                    return "Оранжевый";

                case "4":
                    return "Красный мигающий";

                case "5":
                    return "Зеленый мигающий";

                case "6":
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
