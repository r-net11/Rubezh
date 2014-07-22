using System;
using System.Windows.Data;
using Infrastructure.Models;

namespace JournalModule.Converters
{
	public class JournalColumnTypeToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var journalColumnType = (JournalColumnType)value;
			switch (journalColumnType)
			{
				case JournalColumnType.SubsystemType:
					return "/Controls;component/Images/PC.png";

				case JournalColumnType.UserName:
					return "/Controls;component/Images/PCUser.png";
				
				default:
					return "/Controls;component/Images/blank.png";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}