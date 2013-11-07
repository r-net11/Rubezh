using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using GKProcessor;
using FiresecClient;

namespace GKModule.Converters
{
	public class JournalItemToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var journalItem = (JournalItem)value;
			switch (journalItem.JournalItemType)
			{
				case JournalItemType.Device:
					var device = XManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
					if (device == null)
						return "/Controls;component/Images/blank.png";
					return device.Driver.ImageSource;

				case JournalItemType.Zone:
					return "/Controls;component/Images/zone.png";

				case JournalItemType.Direction:
					return "/Controls;component/Images/Blue_Direction.png";

				case JournalItemType.GK:
					return "/Controls;component/GKIcons/GK.png";

				case JournalItemType.User:
					return "/Controls;component/Images/Chip.png";

				case JournalItemType.System:
					return "/Controls;component/Images/PC.png";

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
