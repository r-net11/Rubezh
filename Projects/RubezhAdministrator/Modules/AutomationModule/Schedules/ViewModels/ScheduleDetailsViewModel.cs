using RubezhAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Automation;
using System.Linq;
using RubezhClient;

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
			var i = 0;
			do { i++; } while (ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSchedules.Any(x => x.Name == Schedule.Name + i));
			Name = Schedule.Name = Schedule.Name + i;
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
			if (ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSchedules.Any(x => x.Name == Name && x.Uid != Schedule.Uid))
			{
				MessageBoxService.ShowWarning("Расписание с таким именем уже существует");
				return false;
			}
			Schedule.Name = Name;
			return base.Save();
		}
	}
}