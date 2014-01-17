using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace JournalModule.ViewModels
{
	public class ArchivePageViewModel : BaseViewModel
	{
		IEnumerable<JournalRecord> JournalRecordsList;
		//IEnumerable<FS2JournalItem> JournalItemsList;

		public ArchivePageViewModel(IEnumerable<JournalRecord> journalRecords)
		{
			JournalRecordsList = journalRecords;
		}

		//public ArchivePageViewModel(IEnumerable<FS2JournalItem> journalItems)
		//{
		//    JournalItemsList = journalItems;
		//}

		public void Create()
		{
			JournalRecords = new List<JournalRecordViewModel>();
			if (JournalRecordsList != null)
			{
				foreach (var journalRecord in JournalRecordsList)
				{
					var journalRecordViewModel = new JournalRecordViewModel(journalRecord);
					JournalRecords.Add(journalRecordViewModel);
				}
			}
			//if (JournalItemsList != null)
			//{
			//    foreach (var journalItem in JournalItemsList)
			//    {
			//        var journalRecordViewModel = new JournalRecordViewModel(journalItem);
			//        JournalRecords.Add(journalRecordViewModel);
			//    }
			//}
		}

		public List<JournalRecordViewModel> JournalRecords { get; private set; }
	}
}