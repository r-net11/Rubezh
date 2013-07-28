using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace JournalModule.ViewModels
{
    public class ArchivePageViewModel : BaseViewModel
    {
		public ArchivePageViewModel(IEnumerable<JournalRecord> journalRecords)
        {
			JournalRecordsList = journalRecords;
        }

		public IEnumerable<JournalRecord> JournalRecordsList { get; private set; }
    }
}