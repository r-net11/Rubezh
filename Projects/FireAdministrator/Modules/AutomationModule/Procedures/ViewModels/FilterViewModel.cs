using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using FiresecAPI;
using FiresecAPI.Journal;

namespace AutomationModule.ViewModels
{
	public class FilterViewModel : BaseViewModel
	{
		public JournalFilter Filter { get; set; }

		public FilterViewModel(JournalFilter filter)
		{
			Filter = filter;
		}

		public string Name
		{
			get { return Filter.Name; }
			set
			{
				Filter.Name = value;
				OnPropertyChanged(() => Name);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public void Update(JournalFilter filter)
		{
			Filter = filter;
			OnPropertyChanged(() => Name);
		}
	}
}