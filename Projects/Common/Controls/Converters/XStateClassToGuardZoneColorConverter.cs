using System;
using System.Windows.Data;
using System.Windows.Media;
using FiresecAPI.GK;

namespace Controls.Converters
{
	public class XStateClassToGuardZoneColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((XStateClass)value)
			{
				case XStateClass.Unknown:
				case XStateClass.DBMissmatch:
				case XStateClass.TechnologicalRegime:
				case XStateClass.ConnectionLost:
				case XStateClass.HasNoLicense:
					return Brushes.DarkGray;

				case XStateClass.On:
					return Brushes.Green;

				case XStateClass.TurningOn:
					return Brushes.LightGreen;

				case XStateClass.AutoOff:
					return Brushes.Gray;

				case XStateClass.Ignore:
					return Brushes.Yellow;

				case XStateClass.Norm:
				case XStateClass.Off:
					return Brushes.Blue;

				case XStateClass.Fire1:
				case XStateClass.Fire2:
				case XStateClass.Attention:
					return Brushes.Red;

				default:
					return Brushes.White;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}