using System;
using System.Globalization;
using System.Windows.Data;

namespace LibraryModule.Converters
{
    public class Transform : IValueConverter
    {
        public double Factor { get; set; }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var intValue = (double) value;
            var a = parameter.ToString().Replace('.', ',');
            Factor = System.Convert.ToDouble(a);
            return intValue * Factor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
