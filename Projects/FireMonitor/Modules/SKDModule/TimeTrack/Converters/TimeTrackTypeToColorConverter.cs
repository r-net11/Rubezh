using System;
using System.Windows.Data;
using System.Windows.Media;
using RubezhAPI.SKD;

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

				case TimeTrackType.Balance:
					return Brushes.Gray;

				case TimeTrackType.Presence:
					return Brushes.Green;

				case TimeTrackType.Absence:
					return Brushes.Red;

				case TimeTrackType.AbsenceInsidePlan:
					return Brushes.Pink;

				case TimeTrackType.PresenceInBrerak:
					return Brushes.DarkGreen;

				case TimeTrackType.Late:
					return Brushes.SkyBlue;

				case TimeTrackType.EarlyLeave:
					return Brushes.LightBlue;

				case TimeTrackType.Overtime:
					return Brushes.Yellow;

				case TimeTrackType.Night:
					return Brushes.YellowGreen;

				case TimeTrackType.DayOff:
					return Brushes.LightGray;

				case TimeTrackType.Holiday:
					return Brushes.Orange;

				case TimeTrackType.DocumentOvertime:
					return Brushes.LightYellow;

				case TimeTrackType.DocumentPresence:
					return Brushes.LightGreen;

				case TimeTrackType.DocumentAbsence:
					return Brushes.LightPink;
			}
			return Brushes.Green;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}