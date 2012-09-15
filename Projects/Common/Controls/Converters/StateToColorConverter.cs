using System;
using System.Windows.Data;
using System.Windows.Media;
using FiresecAPI;

namespace Controls.Converters
{
	public class StateToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((StateType)value)
			{
				case StateType.Fire:
					return Brushes.Red;

				case StateType.Attention:
					return Brushes.Yellow;

				case StateType.Failure:
					return Brushes.Pink;

				case StateType.Service:
					return Brushes.LightGreen;

				case StateType.Off:
					return Brushes.LightYellow;

				case StateType.Unknown:
					return Brushes.Gray;

				case StateType.Info:
					return Brushes.LightBlue;

				case StateType.Norm:
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