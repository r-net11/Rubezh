using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ScheduleViewModel : BaseViewModel
	{
		public const string DefaultName = "<нет>";
		public AutomationSchedule Schedule { get; set; }

		public ScheduleViewModel(AutomationSchedule schedule)
		{
			Schedule = schedule;
		}

		public string Name
		{
			get { return Schedule.Name; }
			set
			{
				Schedule.Name = value;
				OnPropertyChanged("Name");
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => Schedule);
			OnPropertyChanged(() => Name);
		}
	}
}