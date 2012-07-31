using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace AlarmModule.Converters
{
    public class AlarmTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((AlarmType) value)
            {
				case AlarmType.Guard:
					return "DarkRed";

                case AlarmType.Fire:
                    return "Red";

                case AlarmType.Attention:
                    return "Orange";

                case AlarmType.Failure:
                    return "Yellow";

                case AlarmType.Off:
                    return "Wheat";

                case AlarmType.Info:
                    return "SkyBlue";

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