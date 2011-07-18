using System;
using System.Windows.Data;

namespace DevicesModule.Converters
{
    public class ClauseStateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string operation = (string)value;
            switch (operation)
            {
                case "0":
                    return "Включение автоматики";

                case "1":
                    return "Тревога";

                case "2":
                    return "Пожар";

                case "5":
                    return "Внимание";

                case "6":
                    return "Включение модуля пожаротушения";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
