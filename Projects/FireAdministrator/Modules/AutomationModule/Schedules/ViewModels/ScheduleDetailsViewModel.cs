using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

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
			Schedule.Name = Name;
			return base.Save();
		}
	}
}
