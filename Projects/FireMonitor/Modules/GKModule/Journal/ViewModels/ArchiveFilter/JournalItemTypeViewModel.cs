using FiresecAPI;
using FiresecAPI.GK;
using Infrastructure.Common.CheckBoxList;

namespace GKModule.ViewModels
{
	public class JournalItemTypeViewModel : CheckBoxItemViewModel
	{
		public JournalItemTypeViewModel(XJournalItemType journalItemType)
		{
			JournalItemType = journalItemType;
			Name = journalItemType.ToDescription();
		}

		public XJournalItemType JournalItemType { get; private set; }
		public string Name { get; private set; }

		public string ImageSource
		{
			get
			{
				switch (JournalItemType)
				{
					case XJournalItemType.Device:
						return "/Controls;component/GKIcons/RSR2_RM_1.png";

					case XJournalItemType.Zone:
						return "/Controls;component/Images/zone.png";

					case XJournalItemType.Direction:
						return "/Controls;component/Images/Blue_Direction.png";

					case XJournalItemType.GK:
						return "/Controls;component/GKIcons/GK.png";

					case XJournalItemType.GkUser:
						return "/Controls;component/Images/Chip.png";

					case XJournalItemType.System:
						return "/Controls;component/Images/PC.png";

					case XJournalItemType.PumpStation:
						return "/Controls;component/Images/BPumpStation.png";

					case XJournalItemType.MPT:
						return "/Controls;component/Images/BMPT.png";

					case XJournalItemType.Delay:
						return "/Controls;component/Images/Delay.png";

					case XJournalItemType.Pim:
						return "/Controls;component/Images/Pim.png";

					default:
						return null;

				}
			}
		}
	}
}