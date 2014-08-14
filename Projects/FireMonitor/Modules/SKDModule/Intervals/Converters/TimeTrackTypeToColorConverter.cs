using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using FiresecAPI.SKD;
using System.Windows.Media;

namespace SKDModule.Converters
{
	public class TimeTrackTypeToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			TimeTrackType timeTrackType = (TimeTrackType)value;
			switch (timeTrackType)
			{
				case TimeTrackType.None:
					return Brushes.Gray;

				case TimeTrackType.Missed:
					return Brushes.DarkRed;

				case TimeTrackType.AsPlanned:
					return Brushes.White;

				case TimeTrackType.Late:
					return Brushes.Red;

				case TimeTrackType.OutShedule:
					return Brushes.Green;

				case TimeTrackType.DayOff:
					return Brushes.LightGray;

				case TimeTrackType.Holiday:
					return Brushes.GreenYellow;

				case TimeTrackType.Document:
					return Brushes.SkyBlue;
			}
			return Brushes.White;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}