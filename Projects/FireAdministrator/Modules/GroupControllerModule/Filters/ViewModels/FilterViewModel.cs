using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class FilterViewModel : BaseViewModel
	{
		public XJournalFilter JournalFilter { get; set; }

		public FilterViewModel(XJournalFilter journalFilter)
		{
			JournalFilter = journalFilter;
		}

		public void Update()
		{
			OnPropertyChanged("JournalFilter");
		}
	}
}