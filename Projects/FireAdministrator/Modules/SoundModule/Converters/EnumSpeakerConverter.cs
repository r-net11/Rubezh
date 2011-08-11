using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using System.Windows.Data;

namespace SoundsModule.Converters
{
    class EnumSpeakerConverter : IValueConverter
    {
        string DefaultName = "<не задано>";

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Array Value = (Array)value;
            List<string> speakerTypes = new List<string>();
            string type;
            foreach (var speakertype in Value)
            {
                switch ((SpeakerType)speakertype)
                {
                    case SpeakerType.None:
                        type = DefaultName;
                        break;
                    case SpeakerType.Alarm:
                        type = "Тревога";
                        break;
                    case SpeakerType.Attention:
                        type = "Внимание";
                        break;
                    default:
                        type = DefaultName;
                        break;
                }
                speakerTypes.Add(type);
            }
            return speakerTypes;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
