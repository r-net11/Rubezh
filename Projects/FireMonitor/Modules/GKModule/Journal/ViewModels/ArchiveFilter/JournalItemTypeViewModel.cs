using FiresecAPI;
using FiresecAPI.GK;
using Infrastructure.Common.CheckBoxList;

namespace GKModule.ViewModels
{
	public class JournalItemTypeViewModel : CheckBoxItemViewModel
	{
		public JournalItemTypeViewModel(GKJournalObjectType journalObjectType)
		{
			JournalItemType = journalObjectType;
			Name = journalObjectType.ToDescription();
		}

		public GKJournalObjectType JournalItemType { get; private set; }
		public string Name { get; private set; }

		public string ImageSource
		{
			get
			{
				switch (JournalItemType)
				{
					case GKJournalObjectType.Device:
						return "/Controls;component/GKIcons/RSR2_RM_1.png";

					case GKJournalObjectType.Zone:
						return "/Controls;component/Images/Zone.png";

					case GKJournalObjectType.Direction:
						return "/Controls;component/Images/Blue_Direction.png";

					case GKJournalObjectType.GK:
						return "/Controls;component/GKIcons/GK.png";

					case GKJournalObjectType.GkUser:
						return "/Controls;component/Images/Chip.png";

					case GKJournalObjectType.System:
						return "/Controls;component/Images/PC.png";

					case GKJournalObjectType.PumpStation:
						return "/Controls;component/Images/BPumpStation.png";

					case GKJournalObjectType.MPT:
						return "/Controls;component/Images/BMPT.png";

					case GKJournalObjectType.Delay:
						return "/Controls;component/Images/Delay.png";

					case GKJournalObjectType.Pim:
						return "/Controls;component/Images/Pim.png";

					case GKJournalObjectType.GuardZone:
						return "/Controls;component/Images/GuardZone.png";

					default:
						return null;

				}
			}
		}
	}
}