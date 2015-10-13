using ChinaSKDDriverAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class JournalItemViewModel : BaseViewModel
	{
		public SKDJournalItem JournalItem { get; private set; }

		public JournalItemType JournalItemType { get; private set; }

		public JournalItemViewModel(SKDJournalItem journalItem, JournalItemType journalItemType = JournalItemType.Online)
		{
			JournalItem = journalItem;
			JournalItemType = journalItemType;
		}
	}

	public enum JournalItemType
	{
		Online = 0,
		Offline = 1
	}
}