using FiresecAPI;
using FiresecAPI.SKD;
using Infrastructure.Common.CheckBoxList;

namespace JournalModule.ViewModels
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
		public string ImageSource { get; private set; }
	}
}