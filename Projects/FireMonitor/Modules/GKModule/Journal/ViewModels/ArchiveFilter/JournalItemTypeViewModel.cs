using FiresecAPI;
using FiresecAPI.GK;
using Infrastructure.Common.CheckBoxList;

namespace GKModule.ViewModels
{
	public class JournalItemTypeViewModel : CheckBoxItemViewModel
	{
		public JournalItemTypeViewModel(XJournalObjectType journalObjectType)
		{
			JournalItemType = journalObjectType;
			Name = journalObjectType.ToDescription();
		}

		public XJournalObjectType JournalItemType { get; private set; }
		public string Name { get; private set; }

		public string ImageSource
		{
			get
			{
				switch (JournalItemType)
				{
					case XJournalObjectType.Device:
						return "/Controls;component/GKIcons/RSR2_RM_1.png";

					case XJournalObjectType.Zone:
						return "/Controls;component/Images/Zone.png";

					case XJournalObjectType.Direction:
						return "/Controls;component/Images/Blue_Direction.png";

					case XJournalObjectType.GK:
						return "/Controls;component/GKIcons/GK.png";

					case XJournalObjectType.GkUser:
						return "/Controls;component/Images/Chip.png";

					case XJournalObjectType.System:
						return "/Controls;component/Images/PC.png";

					case XJournalObjectType.PumpStation:
						return "/Controls;component/Images/BPumpStation.png";

					case XJournalObjectType.MPT:
						return "/Controls;component/Images/BMPT.png";

					case XJournalObjectType.Delay:
						return "/Controls;component/Images/Delay.png";

					case XJournalObjectType.Pim:
						return "/Controls;component/Images/Pim.png";

					case XJournalObjectType.GuardZone:
						return "/Controls;component/Images/GuardZone.png";

					default:
						return null;

				}
			}
		}
	}
}