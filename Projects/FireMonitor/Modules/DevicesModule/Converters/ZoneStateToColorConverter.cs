using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using FiresecAPI.Models;
using System.Windows.Media;
using FiresecClient;

namespace DevicesModule.Converters
{
	public class ZoneStateToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			ZoneState zoneState = (ZoneState)value;
			if (zoneState.Zone.ZoneType == ZoneType.Guard)
			{
				if (zoneState.StateType == StateType.Norm)
					return Brushes.Blue;

				if (zoneState.IsOnGuard)
					return Brushes.DarkGreen;
			}

			switch (zoneState.StateType)
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
					return Brushes.LightGreen;

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