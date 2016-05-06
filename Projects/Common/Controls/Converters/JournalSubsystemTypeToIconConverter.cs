using StrazhAPI.Journal;
using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class JournalSubsystemTypeToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var subsystemType = (JournalSubsystemType)value;
			switch (subsystemType)
			{
				case JournalSubsystemType.System:
					return "/Controls;component/Images/PC.png";
				case JournalSubsystemType.SKD:
					return "/Controls;component/SKDIcons/Controller.png";
				case JournalSubsystemType.Video:
					return "/Controls;component/Images/Camera.png";
			}
			return "/Controls;component/Images/Blank.png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}