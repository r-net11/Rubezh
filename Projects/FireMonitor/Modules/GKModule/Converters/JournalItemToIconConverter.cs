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
			var journalItem = (GKJournalItem)value;
			switch (journalItem.JournalObjectType)
			{
				case GKJournalObjectType.Device:
					var device = GKManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
					if (device == null)
						return "/Controls;component/Images/Blank.png";
					return device.Driver.ImageSource;

				case GKJournalObjectType.Zone:
					return "/Controls;component/Images/Zone.png";

				case GKJournalObjectType.Direction:
					return "/Controls;component/Images/Blue_Direction.png";

				case GKJournalObjectType.Delay:
					return "/Controls;component/Images/Delay.png";

				case GKJournalObjectType.Pim:
					return "/Controls;component/Images/Pim.png";

				case GKJournalObjectType.GkUser:
					return "/Controls;component/Images/Chip.png";

				case GKJournalObjectType.PumpStation:
					return "/Controls;component/Images/BPumpStation.png";

				case GKJournalObjectType.MPT:
					return "/Controls;component/Images/BMPT.png";

				case GKJournalObjectType.GuardZone:
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