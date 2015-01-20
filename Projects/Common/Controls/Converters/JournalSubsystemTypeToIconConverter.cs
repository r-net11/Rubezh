using System;
using System.Windows.Data;
using FiresecAPI.Journal;

namespace Controls.Converters
{
	public class JournalSubsystemTypeToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var subsystemType = (JournalSubsystemType)value;
			switch(subsystemType)
			{
				case JournalSubsystemType.System:
					return "PC";
				case JournalSubsystemType.GK:
					return "Chip";
				case JournalSubsystemType.SKD:
					return "/Controls;component/SKDIcons/Controller.png";
				case JournalSubsystemType.Video:
					return "Camera";
			}
			return "Blank";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}