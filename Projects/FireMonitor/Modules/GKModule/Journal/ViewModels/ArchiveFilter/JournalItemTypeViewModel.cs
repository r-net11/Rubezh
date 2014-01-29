using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using GKProcessor;
using FiresecAPI;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class JournalItemTypeViewModel : CheckBoxItemViewModel
	{
		public JournalItemTypeViewModel(JournalItemType journalItemType)
		{
			JournalItemType = journalItemType;
			Name = journalItemType.ToDescription();
		}

		public JournalItemType JournalItemType { get; private set; }
		public string Name { get; private set; }

		public string ImageSource
		{
			get
			{
				switch (JournalItemType)
				{
					case JournalItemType.Device:
						return "/Controls;component/GKIcons/RSR2_RM_1.png";

					case JournalItemType.Zone:
						return "/Controls;component/Images/zone.png";

					case JournalItemType.Direction:
						return "/Controls;component/Images/Blue_Direction.png";

					case JournalItemType.GK:
						return "/Controls;component/GKIcons/GK.png";

					case JournalItemType.GkUser:
						return "/Controls;component/Images/Chip.png";

					case JournalItemType.System:
						return "/Controls;component/Images/PC.png";

					case JournalItemType.PumpStation:
						return "/Controls;component/Images/BPumpStation.png";

					case JournalItemType.Delay:
						return "/Controls;component/Images/Delay.png";

					case JournalItemType.Pim:
						return "/Controls;component/Images/Pim.png";

					default:
						return "";

				}
			}
		}
	}
}