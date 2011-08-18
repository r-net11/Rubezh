using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using FiresecAPI.Models;
using System.IO;
using SoundsModule.ViewModels;

namespace SoundsModule.Converters
{
    class SoundToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string Value = (string)value;
            switch (Value)
            {
                case null:
                    return SoundViewModel.DefaultName;
                default:
                    if (AvailableSounds.Any(x => x == Value))
                    {
                        return Value;
                    }
                    else
                    {
                        return SoundViewModel.DefaultName;
                    }
            }
                    
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string Value = (string)value;
            switch (Value)
            {
                case SoundViewModel.DefaultName:
                    return string.Empty;
                default:
                    return Value;
            }
            throw new NotImplementedException();
        }

        List<string> AvailableSounds
        {
            get { return FiresecClient.FileHelper.SoundsList; }
        }
    }
}
