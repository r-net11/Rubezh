using System;
using System.Windows.Data;
using FiresecAPI.Models;
using System.Collections.Generic;
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
                    return SoundViewModel.DefaultName;
                case SpeakerType.Alarm:
                    return "Тревога";
                case SpeakerType.Attention:
                    return "Внимание";
                default:
                    return "<нет>";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (SpeakerType)value;
            
        }
    }
}