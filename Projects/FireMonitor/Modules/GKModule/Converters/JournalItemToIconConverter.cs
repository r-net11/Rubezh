using System;
using System.Linq;
using System.Windows.Data;
using FiresecAPI.GK;
using FiresecClient;

namespace GKModule.Converters
{
	public class JournalItemToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var journalItem = (XJournalItem)value;
			switch (journalItem.JournalObjectType)
			{
				case XJournalObjectType.Device:
					var device = XManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
					if (device == null)
						return "/Controls;component/Images/Blank.png";
					return device.Driver.ImageSource;

				case XJournalObjectType.Zone:
					return "/Controls;component/Images/Zone.png";

				case XJournalObjectType.Direction:
					return "/Controls;component/Images/Blue_Direction.png";

				case XJournalObjectType.Delay:
					return "/Controls;component/Images/Delay.png";

				case XJournalObjectType.Pim:
					return "/Controls;component/Images/Pim.png";

				case XJournalObjectType.GkUser:
					return "/Controls;component/Images/Chip.png";

				case XJournalObjectType.PumpStation:
					return "/Controls;component/Images/BPumpStation.png";

				case XJournalObjectType.MPT:
					return "/Controls;component/Images/BMPT.png";

				case XJournalObjectType.GuardZone:
					return "/Controls;component/Images/GuardZone.png";

				default:
					return "/Controls;component/Images/Blank.png";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}