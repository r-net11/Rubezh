using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace SoundsModule.Converters
{
    class SpeakerTypeToStringConverter : IValueConverter
    {
        string DefaultName = "<нет>";

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SpeakerType Value = (SpeakerType)value;
            switch (Value)
            {
                case SpeakerType.None:
                    return DefaultName;
                case SpeakerType.Alarm:
                    return "Тревога";
                case SpeakerType.Attention:
                    return "Внимание";
                default:
                    return DefaultName;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}