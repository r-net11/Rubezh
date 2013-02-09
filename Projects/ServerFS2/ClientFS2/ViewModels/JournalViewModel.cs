using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2;

namespace ClientFS2.ViewModels
{
	public class JournalViewModel : DialogViewModel
	{
		public JournalViewModel(List<JournalItem> journalItems)
		{
			Title = "Журнал событий";

			JournalItems = new ObservableCollection<JournalItem>();
			foreach (var journalItem in journalItems)
			{
				JournalItems.Add(journalItem);
			}
		}
		public ObservableCollection<JournalItem> JournalItems { get; set; }
	}
}