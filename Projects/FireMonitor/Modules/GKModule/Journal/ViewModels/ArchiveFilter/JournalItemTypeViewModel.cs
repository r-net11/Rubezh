using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using GKProcessor;
using FiresecAPI;

namespace GKModule.ViewModels
{
	public class JournalItemTypeViewModel : BaseViewModel
	{
		public JournalItemTypeViewModel(JournalItemType journalItemType)
		{
			JournalItemType = journalItemType;
			Name = journalItemType.ToDescription();
		}

		public JournalItemType JournalItemType { get; private set; }
		public string Name { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}

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

					case JournalItemType.User:
						return "/Controls;component/Images/Chip.png";

					case JournalItemType.System:
						return "/Controls;component/Images/PC.png";

					default:
						return "";

				}
			}
		}
	}
}