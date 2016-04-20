using System.Collections.Generic;
using System.Collections.ObjectModel;
using FS2Api;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using System.Linq;

namespace AdministratorTestClientFS2.ViewModels
{
	public class JournalViewModel : DialogViewModel
	{
		public JournalViewModel(List<FS2JournalItem> journalItems, string name)
		{
			Title = "Журнал событий " + name;

			JournalItems = new ObservableCollection<FS2JournalItem>();
			foreach (var journalItem in journalItems)
			{
				JournalItems.Add(journalItem);
			}
		}
		public ObservableCollection<FS2JournalItem> JournalItems { get; set; }

		FS2JournalItem _selectedJournalItem;
		public FS2JournalItem SelectedJournalItem
		{
			get { return _selectedJournalItem; }
			set
			{
				_selectedJournalItem = value;
				OnPropertyChanged("SelectedJournalItem");
			}
		}
	}
}