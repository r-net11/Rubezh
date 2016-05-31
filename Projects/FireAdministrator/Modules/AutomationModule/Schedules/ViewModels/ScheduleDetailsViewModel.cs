using StrazhAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using Localization.Automation.Common;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ScheduleDetailsViewModel : SaveCancelDialogViewModel
	{
		public AutomationSchedule Schedule { get; private set; }
		public ScheduleDetailsViewModel(AutomationSchedule schedule)
		{
            Title = CommonViewModel.ScheduleDetails_Properties;
			Schedule = schedule;
			Name = Schedule.Name;
		}

		public ScheduleDetailsViewModel()
		{
            Title = CommonViewModel.ScheduleDetails_Add;
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
				MessageBoxService.ShowWarning(CommonResources.SaveEmpty);
				return false;
			}
			Schedule.Name = Name;
			return base.Save();
		}
	}
}