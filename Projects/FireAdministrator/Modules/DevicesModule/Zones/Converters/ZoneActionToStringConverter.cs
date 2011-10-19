using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace DevicesModule.Converters
{
    public class ZoneActionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ZoneActionType)
            {
                var zoneActionType = (ZoneActionType)value;
                switch (zoneActionType)
                {
                    case ZoneActionType.Set:
                        return "Поставить на охрану";

                    case ZoneActionType.Unset:
                        return "Снять с охраны";
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
