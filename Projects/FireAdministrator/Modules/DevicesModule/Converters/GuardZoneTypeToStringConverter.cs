using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace DevicesModule.Converters
{
    public class GuardZoneTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            GuardZoneType guardZoneType = (GuardZoneType)value;
            switch (guardZoneType)
            {
                case GuardZoneType.Ordinary:
                    return "Обычная";

                case GuardZoneType.Passby:
                    return "Проходная";

                case GuardZoneType.Delay:
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