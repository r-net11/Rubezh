using ChinaSKDDriverAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class JournalItemViewModel : BaseViewModel
	{
		public SKDJournalItem JournalItem { get; private set; }

		public JournalItemViewModel(SKDJournalItem journalItem)
		{
			JournalItem = journalItem;
		}
	}
}