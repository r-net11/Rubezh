using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace JournalModule.ViewModels
{
    public class ArchivePageViewModel : BaseViewModel
    {
		public ArchivePageViewModel(List<JournalRecordViewModel> journalRecords)
        {
			JournalRecords = journalRecords;
        }

		public List<JournalRecordViewModel> JournalRecords { get; private set; }
    }
}