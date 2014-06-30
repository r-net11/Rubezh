using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class FilterViewModel : BaseViewModel
	{
		public AutomationFilter Filter { get; set; }

		public FilterViewModel(AutomationFilter filter)
		{
			Filter = filter;
		}

		public string Name
		{
			get { return Filter.Name; }
			set
			{
				Filter.Name = value;
				OnPropertyChanged("Name");
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public void Update(AutomationFilter filter)
		{
			Filter = filter;
			OnPropertyChanged("Name");
		}
	}
}