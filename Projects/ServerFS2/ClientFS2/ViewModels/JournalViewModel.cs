using System.Collections.Generic;
using System.Collections.ObjectModel;
using FS2Api;
using Infrastructure.Common.Windows.ViewModels;

namespace ClientFS2.ViewModels
{
	public class JournalViewModel : DialogViewModel
	{
		public JournalViewModel(List<FS2JournalItem> journalItems)
		{
			Title = "Журнал событий";

			JournalItems = new ObservableCollection<FS2JournalItem>();
			foreach (var journalItem in journalItems)
			{
				JournalItems.Add(journalItem);
			}
		}
		public ObservableCollection<FS2JournalItem> JournalItems { get; set; }
	}
}