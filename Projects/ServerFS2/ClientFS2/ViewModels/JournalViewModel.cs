using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2;
using FS2Api;

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