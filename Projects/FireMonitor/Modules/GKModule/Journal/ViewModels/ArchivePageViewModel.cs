using System.Collections.Generic;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ArchivePageViewModel : BaseViewModel
	{
		IEnumerable<JournalItem> JournalItemsList;

		public ArchivePageViewModel(IEnumerable<JournalItem> journalItems)
		{
			JournalItemsList = journalItems;
		}

		public void Create()
		{
			JournalItems = new List<JournalItemViewModel>();
			if (JournalItemsList != null)
			{
				foreach (var journalItem in JournalItemsList)
				{
					var journalRecordViewModel = new JournalItemViewModel(journalItem);
					JournalItems.Add(journalRecordViewModel);
				}
			}
			//if (JournalItemsList != null)
			//{
			//	foreach (var journalItem in JournalItemsList)
			//	{
			//		var journalRecordViewModel = new JournalRecordViewModel(journalItem);
			//		JournalRecords.Add(journalRecordViewModel);
			//	}
			//}
		}

		public List<JournalItemViewModel> JournalItems { get; private set; }
	}
}