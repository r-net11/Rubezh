using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using FiresecAPI.Models;
using SoundsModule.ViewModels;
using System.IO;

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
                    return DownloadHelper.DefaultName;
                default:
                    if (AvailableSounds.Any(x => x == Value))
                    {
                        return Value;
                    }
                    else
                    {
                        return DownloadHelper.DefaultName;
                    }
            }
                    
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string Value = (string)value;
            switch (Value)
            {
                case DownloadHelper.DefaultName:
                    return string.Empty;
                default:
                    return Value;
            }
            //throw new NotImplementedException();
        }

        List<string> AvailableSounds
        {
            get
            {
                List<string> fileNames = new List<string>();
                fileNames.Add(DownloadHelper.DefaultName);
                foreach (string str in Directory.GetFiles(DownloadHelper.CurrentDirectory))
                {
                    fileNames.Add(Path.GetFileName(str));
                }
                return fileNames;
            }
        }
    }
}
