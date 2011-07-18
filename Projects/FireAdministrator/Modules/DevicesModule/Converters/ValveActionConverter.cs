using System;
using System.Windows.Data;

namespace DevicesModule.Converters
{
    public class ValveActionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string action = (string)value;
            switch (action)
            {
                case "0":
                    return "Закрытие";

                case "1":
                    return "Открытие";
            }
            return "Закрытие";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
