using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;

namespace GKModule.ViewModels
{
	public class ScheduleDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKSchedule Schedule;
		public bool CanChangeScheduleType { get; private set; }

		public ScheduleDetailsViewModel(GKSchedule schedule = null)
		{
			if (schedule == null)
			{
				Title = "Создание нового графика работ";
				CanChangeScheduleType = true;

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

			ScheduleTypes = new ObservableCollection<GKScheduleType>(Enum.GetValues(typeof(GKScheduleType)).Cast<GKScheduleType>());
			SelectedScheduleType = Schedule.ScheduleType;

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
			StartDateTime = Schedule.StartDateTime;
			HoursPeriod = Schedule.HoursPeriod;
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

		DateTime _startDateTime;
		public DateTime StartDateTime
		{
			get { return _startDateTime; }
			set
			{
				_startDateTime = value;
				OnPropertyChanged(() => StartDateTime);
			}
		}

		int _hoursPeriod;
		public int HoursPeriod
		{
			get { return _hoursPeriod; }
			set
			{
				_hoursPeriod = value;
				OnPropertyChanged(() => HoursPeriod);
			}
		}

		bool _canChangeHoursPeriod;
		public bool CanChangeHoursPeriod
		{
			get { return _canChangeHoursPeriod; }
			set
			{
				_canChangeHoursPeriod = value;
				OnPropertyChanged(() => CanChangeHoursPeriod);
			}
		}

		public ObservableCollection<GKScheduleType> ScheduleTypes { get; private set; }

		GKScheduleType _selectedScheduleType;
		public GKScheduleType SelectedScheduleType
		{
			get { return _selectedScheduleType; }
			set
			{
				_selectedScheduleType = value;
				CanChangeHoursPeriod = value == GKScheduleType.Custom;
				OnPropertyChanged(() => SelectedScheduleType);
			}
		}

		protected override bool Save()
		{
			if (No <= 0 || No >= 255)
			{
				MessageBoxService.Show("Номер должен быть задан в диапазоне от 1 до 255");
				return false;
			}
			if (Schedule.No != No && GKManager.DeviceConfiguration.Schedules.Any(x => x.No == No))
			{
				MessageBoxService.Show("График с таким номером уже существует");
				return false;
			}

			if (SelectedScheduleType == GKScheduleType.Custom && HoursPeriod <= 0)
			{
				MessageBoxService.Show("Величина периода должна быть положительным числом");
				return false;
			}

			Schedule.No = No;
			Schedule.Name = Name;
			Schedule.Description = Description;
			Schedule.StartDateTime = StartDateTime;
			if (SelectedScheduleType == GKScheduleType.Custom)
			{
				Schedule.HoursPeriod = HoursPeriod;
			}
			else
			{
				Schedule.HoursPeriod = 0;
			}
			Schedule.ScheduleType = SelectedScheduleType;
			return base.Save();
		}
	}
}