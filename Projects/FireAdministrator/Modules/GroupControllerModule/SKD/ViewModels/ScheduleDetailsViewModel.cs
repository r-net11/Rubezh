using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class ScheduleDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKSchedule Schedule;

		public ScheduleDetailsViewModel(GKSchedule schedule = null)
		{
			if (schedule == null)
			{
				Title = "Создание нового графика работ";

				Schedule = new GKSchedule()
				{
					Name = "Новый график работ",
					No = 1
				};
				if (GKManager.DeviceConfiguration.Schedules.Count != 0)
					Schedule.No = (ushort)(GKManager.DeviceConfiguration.Schedules.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства графика работ: {0}", schedule.PresentationName);
				Schedule = schedule;
			}

			CopyProperties();

			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingSchedule in GKManager.DeviceConfiguration.Schedules)
			{
				availableNames.Add(existingSchedule.Name);
				availableDescription.Add(existingSchedule.Description);
			}
			AvailableNames = new ObservableCollection<string>(availableNames);
			AvailableDescription = new ObservableCollection<string>(availableDescription);
		}

		void CopyProperties()
		{
			No = Schedule.No;
			Name = Schedule.Name;
			Description = Schedule.Description;
		}

		int _no;
		public int No
		{
			get { return _no; }
			set
			{
				_no = value;
				OnPropertyChanged(() => No);
			}
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

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		public ObservableCollection<string> AvailableNames { get; private set; }
		public ObservableCollection<string> AvailableDescription { get; private set; }

		protected override bool Save()
		{
			if (Schedule.No != No && GKManager.DeviceConfiguration.Schedules.Any(x => x.No == No))
			{
				MessageBoxService.Show("Зона с таким номером уже существует");
				return false;
			}

			Schedule.No = No;
			Schedule.Name = Name;
			Schedule.Description = Description;
			return base.Save();
		}
	}
}