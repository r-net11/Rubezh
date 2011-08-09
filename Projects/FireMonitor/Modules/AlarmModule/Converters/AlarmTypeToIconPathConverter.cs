using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace AlarmModule.Converters
{
    public class AlarmTypeToIconPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            AlarmType alarmType = (AlarmType) value;
            switch (alarmType)
            {
                case AlarmType.Fire:
                    return "/Controls;component/Images/Alarm_Main_1_Fire.png";

                case AlarmType.Attention:
                    return "/Controls;component/Images/Alarm_main_2_Attention.png";

                case AlarmType.Failure:
                    return "/Controls;component/Images/Alarm_main_3_Failure.png";

                case AlarmType.Off:
                    return "/Controls;component/Images/Alarm_main_4_Off.png";

                case AlarmType.Info:
                    return "/Controls;component/Images/Alarm_main_5_Info.png";

                case AlarmType.Service:
                    return "/Controls;component/Images/Alarm_main_6_Service.png";

                case AlarmType.Auto:
                    return "/Controls;component/Images/Alarm_main_7_Auto.png";

                default:
                    return "/Controls;component/Images/Alarm_main_3_Failure.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}