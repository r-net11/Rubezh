using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace AutomationModule.ViewModels
{
	public class ScheduleDetailsViewModel : SaveCancelDialogViewModel
	{
		public AutomationSchedule Schedule { get; private set; }
		public ScheduleDetailsViewModel(AutomationSchedule schedule)
		{
            Title = Resources.Language.Schedules.ViewModels.ScheduleDetailsViewModel.Properties_Title;
			Schedule = schedule;
			Name = Schedule.Name;
		}

		public ScheduleDetailsViewModel()
		{
            Title = Resources.Language.Schedules.ViewModels.ScheduleDetailsViewModel.Add_Title;
			Schedule = new AutomationSchedule();
			Name = Schedule.Name;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.ShowWarning(Resources.Language.Schedules.ViewModels.ScheduleDetailsViewModel.EmptyName);
				return false;
			}
			Schedule.Name = Name;
			return base.Save();
		}
	}
}