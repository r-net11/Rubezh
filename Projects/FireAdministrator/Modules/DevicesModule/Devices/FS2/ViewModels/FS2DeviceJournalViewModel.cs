using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FS2Api;
using System.Collections.ObjectModel;

namespace DevicesModule.ViewModels
{
	public class FS2DeviceJournalViewModel : DialogViewModel
	{
		public FS2DeviceJournalViewModel(List<FS2JournalItem> journalItems)
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