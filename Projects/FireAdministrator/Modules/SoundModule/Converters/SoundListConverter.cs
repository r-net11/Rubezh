using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using FiresecAPI.Models;
using System.IO;

namespace SoundsModule.Converters
{
    class SoundListConverter : IValueConverter
    {
        const string DefaultName = "<не задано>";

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<string> Value = (List<string>)value;
            List<string> newValue = new List<string>();
            newValue.Add(DefaultName);
            newValue.AddRange(Value);
            return newValue;
                    
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
