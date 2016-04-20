using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Common;

namespace Resurs.ViewModels
{
	public class FilterEventsViewModel : DialogViewModel
	{
		public FilterEventsViewModel(Filter filter)
		{
			FilterEventViewModels = new ObservableCollection<FilterEventViewModel>(Enum.GetValues(typeof(JournalType)).Cast<JournalType>().Select(x => new FilterEventViewModel(x)));
			FilterEventViewModels.ForEach(x =>
				{
					x.IsChecked = filter.JournalTypes.Any(y => y == x.JournalType);
				});
		}

		public ObservableCollection <FilterEventViewModel> FilterEventViewModels { get; set; }

		public List<JournalType> GetJournalTypes()
		{
			var journalType = new List<JournalType>();
			FilterEventViewModels.ForEach(x=> 
			{
				if (x.IsChecked)
					journalType.Add(x.JournalType);
			});
			return journalType;
		}
	}
}