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
					return "PC";

				case JournalColumnType.UserName:
					return "PCUser";
				
				default:
					return "blank";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}