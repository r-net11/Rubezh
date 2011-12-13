using System;
using System.Windows.Data;
using System.Windows.Media;
using FiresecAPI.Models;

namespace Controls.Converters
{
    public class ZoneStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ZoneState zoneState = (ZoneState)value;
            switch (zoneState.StateType)
            {
                case StateType.Fire:
                    return Brushes.Red;

                case StateType.Attention:
                    return Brushes.Yellow;

                case StateType.Failure:
                    return Brushes.Pink;

                case StateType.Service:
                    return Brushes.Yellow;

                case StateType.Off:
                    return Brushes.Red;

                case StateType.Unknown:
                    return Brushes.Gray;

                case StateType.Info:
                    if (zoneState.RevertColorsForGuardZone)
                        return Brushes.Green;
                    return Brushes.LightBlue;

                case StateType.Norm:
                    if (zoneState.RevertColorsForGuardZone)
                        return Brushes.Blue;
                    return Brushes.Green;

                default:
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}