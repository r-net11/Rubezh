using StrazhAPI.GK;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Controls.Converters
{
	public class XStateClassToZoneColorConverter : IValueConverter
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
				case XStateClass.Fire1:
					return Brushes.Red;

				case XStateClass.Attention:
					return Brushes.Orange;

				case XStateClass.Ignore:
					return Brushes.Yellow;

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