using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using SoundsModule.ViewModels;

namespace SoundsModule.Converters
{
    class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string Value = (string)value;
            switch (Value)
            {
                case "Нет":
                    return DownloadHelper.DefaultName;
                case "Тревога":
                    return "Тревога";
                case "Внимание":
                    return "Внимание";
                default:
                    return DownloadHelper.DefaultName;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
