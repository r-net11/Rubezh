using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using FiresecClient.Models;

namespace AlarmModule.Converters
{
    public class AlarmTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            AlarmType alarmType = (AlarmType)value;
            switch (alarmType)
            {
                case AlarmType.Fire:
                    return "Red";

                case AlarmType.Attention:
                    return "Orange";

                case AlarmType.Failure:
                    return "Yellow";

                case AlarmType.Off:
                    return "Wheat";

                case AlarmType.Info:
                    return "Green";

                case AlarmType.Service:
                    return "SkyBlue";

                case AlarmType.Auto:
                    return "GreenYellow";

                default:
                    return "Transparent";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
