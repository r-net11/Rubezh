using System;
using System.Windows.Data;
using FiresecAPI.Models;
using SoundsModule.ViewModels;
using SoundsModule.ViewModels;

namespace SoundsModule.Converters
{
    class SpeakerTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SpeakerType Value = (SpeakerType)value;
            switch (Value)
            {
                case SpeakerType.None:
                    return DownloadHelper.DefaultName;
                case SpeakerType.Alarm:
                    return "Тревога";
                case SpeakerType.Attention:
                    return "Внимание";
                default:
                    return DownloadHelper.DefaultName;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var Value = (string)value;
            switch (Value)
            {
                case SoundViewModel.DefaultName:
                    return SpeakerType.None;
                case "Тревога":
                    return SpeakerType.Alarm;
                case "Внимание":
                    return SpeakerType.Attention;
                default:
                    return SpeakerType.None;
            }
        }
    }
}