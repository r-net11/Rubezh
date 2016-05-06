using StrazhAPI.GK;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Controls.Converters
{
	public class XStateClassToColorConverter2 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((XStateClass)value)
			{
				case XStateClass.Unknown:
				case XStateClass.DBMissmatch:
				case XStateClass.ConnectionLost:
				case XStateClass.TechnologicalRegime:
				case XStateClass.HasNoLicense:
					return Brushes.Gray;

				case XStateClass.Fire2:
					return Brushes.Red;

				case XStateClass.Fire1:
					return Brushes.Red;

				case XStateClass.Attention:
					return Brushes.Orange;

				//case XStateClass.Failure:
				//	return Brushes.Pink;

				//case XStateClass.Service:
				//	return Brushes.Yellow;

				case XStateClass.Ignore:
					return Brushes.Yellow;

				case XStateClass.On:
				case XStateClass.TurningOn:
					return Brushes.LightBlue;

				case XStateClass.AutoOff:
					return Brushes.Yellow;

				//case XStateClass.Test:
				//	return Brushes.Green;

				case XStateClass.Off:
				case XStateClass.TurningOff:
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