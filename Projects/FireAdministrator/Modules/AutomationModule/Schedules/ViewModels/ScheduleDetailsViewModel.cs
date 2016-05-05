using StrazhAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace AutomationModule.ViewModels
{
	public class ScheduleDetailsViewModel : SaveCancelDialogViewModel
	{
		public AutomationSchedule Schedule { get; private set; }
		public ScheduleDetailsViewModel(AutomationSchedule schedule)
		{
			Title = "Свойства элемента расписания";
			Schedule = schedule;
			Name = Schedule.Name;
		}

		public ScheduleDetailsViewModel()
		{
			Title = "Добавить элемент расписания";
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
				MessageBoxService.ShowWarning("Название не может быть пустым");
				return false;
			}
			Schedule.Name = Name;
			return base.Save();
		}
	}
}