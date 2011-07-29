using System;
using System.Windows.Data;
using System.Windows.Media;
using FiresecAPI.Models;

namespace JournalModule.Converters
{
    public class StateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((StateType) value)
            {
                case StateType.Fire:
                    return Brushes.Red;

                case StateType.Failure:
                    return Brushes.LightPink;

                case StateType.Info:
                    return Brushes.Transparent;

                case StateType.No:
                    return Brushes.Transparent;

                case StateType.Norm:
                    return Brushes.Green;

                case StateType.Off:
                    return Brushes.Red;

                case StateType.Service:
                    return Brushes.Yellow;

                case StateType.Unknown:
                    return Brushes.Gray;

                case StateType.Attention:
                    return Brushes.Yellow;

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
