using System;
using System.Windows.Data;
using System.Windows.Media;
using FiresecAPI.SKD;

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
					return new SolidColorBrush(Colors.Gray);

				case TimeTrackType.Balance:
					return new SolidColorBrush(Colors.Gray);

				case TimeTrackType.Presence:
					return new SolidColorBrush(Colors.Green);

				case TimeTrackType.Absence:
					return new SolidColorBrush(Colors.Red);


				case TimeTrackType.AbsenceInsidePlan:
					return new SolidColorBrush(Colors.Pink);

				case TimeTrackType.Late:
					return new SolidColorBrush(Colors.SkyBlue);

				case TimeTrackType.EarlyLeave:
					return new SolidColorBrush(Colors.LightBlue);

				case TimeTrackType.Overtime:
					return new SolidColorBrush(Colors.Yellow);

				case TimeTrackType.Night:
					return new SolidColorBrush(Colors.YellowGreen);

				case TimeTrackType.DayOff:
					return new SolidColorBrush(Colors.LightGray);

				case TimeTrackType.Holiday:
					return new SolidColorBrush(Colors.DarkGray);

				case TimeTrackType.DocumentOvertime:
					return new SolidColorBrush(Colors.LightYellow);

				case TimeTrackType.DocumentPresence:
					return new SolidColorBrush(Colors.LightGreen);

				case TimeTrackType.DocumentAbsence:
					return new SolidColorBrush(Colors.LightPink);
			}

			return new SolidColorBrush(Colors.Green);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}