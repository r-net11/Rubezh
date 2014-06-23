using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using ChinaSKDDriverAPI;

namespace ControllerSDK.ViewModels
{
	public class JournalItemViewModel : BaseViewModel
	{
		public SKDJournalItem JournalItem { get; private set; }

		public JournalItemViewModel(SKDJournalItem journalItem)
		{
			JournalItem = journalItem;
		}


	}
}