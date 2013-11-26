using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using GKProcessor;
using FiresecClient;
using XFiresecAPI;
using Infrastructure.Models;

namespace GKModule.Converters
{
	public class JournalColumnTypeToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var journalColumnType = (JournalColumnType)value;
			switch (journalColumnType)
			{
				case JournalColumnType.GKIpAddress:
					return "/Controls;component/GKIcons/GK.png";

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