using System;
using System.Windows.Data;
using System.Windows.Media;
using XFiresecAPI;

namespace Controls.Converters
{
    public class XStateClassToZoneColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((XStateClass)value)
            {
                case XStateClass.Fire2:
                    return Brushes.Red;

                case XStateClass.Fire1:
                    return Brushes.Red;

                case XStateClass.Attention:
                    return Brushes.Yellow;

                case XStateClass.Ignore:
                    return Brushes.Yellow;

                case XStateClass.Unknown:
                    return Brushes.Gray;

                case XStateClass.Norm:
                    return Brushes.Green;

                default:
                    return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}