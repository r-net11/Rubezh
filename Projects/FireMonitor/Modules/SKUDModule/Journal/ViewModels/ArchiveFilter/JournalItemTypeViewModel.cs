using FiresecAPI;
using Infrastructure.Common.CheckBoxList;
using XFiresecAPI;

namespace SKDModule.ViewModels
{
	public class JournalItemTypeViewModel : CheckBoxItemViewModel
	{
		public JournalItemTypeViewModel(SKDJournalItemType journalItemType)
		{
			JournalItemType = journalItemType;
			Name = journalItemType.ToDescription();
		}

		public SKDJournalItemType JournalItemType { get; private set; }
		public string Name { get; private set; }

		public string ImageSource
		{
			get
			{
				switch (JournalItemType)
				{
					case SKDJournalItemType.System:
						return "/Controls;component/Images/PC.png";

					case SKDJournalItemType.Reader:
						return "/Controls;component/GKIcons/RSR2_RM_1.png";

					case SKDJournalItemType.Controller:
						return "/Controls;component/GKIcons/GK.png";

					default:
						return null;

				}
			}
		}
	}
}