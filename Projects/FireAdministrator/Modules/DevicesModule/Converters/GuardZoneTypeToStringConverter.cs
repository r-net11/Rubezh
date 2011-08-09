using System;
using System.Windows.Data;

namespace DevicesModule.Converters
{
    public class GuardZoneTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string stringType = (string) value;
            switch (stringType)
            {
                case "0":
                    return "Обычная";

                case "1":
                    return "Проходная";

                case "2":
                    return "С задержкой входа/выхода";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}