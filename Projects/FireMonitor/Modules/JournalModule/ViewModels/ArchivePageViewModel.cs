using System.Collections.Generic;
using StrazhAPI.Journal;
using Infrastructure.Common.Windows.ViewModels;

namespace JournalModule.ViewModels
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
					var journalItemViewModel = new JournalItemViewModel(journalItem);
					JournalItems.Add(journalItemViewModel);
				}
			}
		}

		public List<JournalItemViewModel> JournalItems { get; private set; }
	}
}