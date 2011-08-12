using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using FiresecAPI.Models;
using System.IO;

namespace SoundsModule.Converters
{
    class SoundToStringConverter : IValueConverter
    {
        const string DefaultName = "<нет>";

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string Value = (string)value;
            switch (Value)
            {
                case null:
                    return DefaultName;
                default:
                    if (AvailableSounds.Any(x => x == Value))
                    {
                        return Value;
                    }
                    else
                    {
                        return DefaultName;
                    }
            }
                    
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string Value = (string)value;
            switch (Value)
            {
                case DefaultName:
                    return string.Empty;
                default:
                    return Value;
            }
            //throw new NotImplementedException();
        }

        List<string> AvailableSounds
        {
            get { return FiresecClient.FiresecManager.FileHelper.SoundsList; }
        }
    }
}
