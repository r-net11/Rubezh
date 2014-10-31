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
		public SchedulePartsViewModel SchedulePartsViewModel { get; private set; }
		public GKSchedule Schedule;
		public bool CanChangeScheduleType { get; private set; }

		public ScheduleDetailsViewModel(GKSchedule schedule = null)
		{
			if (schedule == null)
			{
				Title = "Создание нового графика доступа";
				CanChangeScheduleType = true;

				Schedule = new GKSchedule()
				{
					Name = "Новый график доступа",
					No = 1
				};
				if (GKManager.DeviceConfiguration.Schedules.Count != 0)
					Schedule.No = (ushort)(GKManager.DeviceConfiguration.Schedules.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства графика доступа: {0}", schedule.PresentationName);
				Schedule = schedule;
			}

			ScheduleTypes = new ObservableCollection<GKScheduleType>(Enum.GetValues(typeof(GKScheduleType)).Cast<GKScheduleType>());
			SelectedScheduleType = Schedule.ScheduleType;

			SchedulePeriodTypes = new ObservableCollection<GKSchedulePeriodType>(Enum.GetValues(typeof(GKSchedulePeriodType)).Cast<GKSchedulePeriodType>());
			SelectedSchedulePeriodType = Schedule.SchedulePeriodType;

			Holidays = new ObservableCollection<GKSchedule>();
			foreach (var holidaySchedule in GKManager.DeviceConfiguration.Schedules)
			{
				if (holidaySchedule.ScheduleType == GKScheduleType.Holiday)
				{
					Holidays.Add(holidaySchedule);
				}
			}
			SelectedHoliday = Holidays.FirstOrDefault(x => x.No == Schedule.HolidayScheduleNo);

			WorkHolidays = new ObservableCollection<GKSchedule>();
			foreach (var workHolidaySchedule in GKManager.DeviceConfiguration.Schedules)
			{
				if (workHolidaySchedule.ScheduleType == GKScheduleType.WorkHoliday)
				{
					WorkHolidays.Add(workHolidaySchedule);
				}
			}
			SelectedWorkHoliday = Holidays.FirstOrDefault(x => x.No == Schedule.WorkHolidayScheduleNo);

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

			SchedulePartsViewModel = new SchedulePartsViewModel(Schedule);
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

		public ObservableCollection<GKScheduleType> ScheduleTypes { get; private set; }

		GKScheduleType _selectedScheduleType;
		public GKScheduleType SelectedScheduleType
		{
			get { return _selectedScheduleType; }
			set
			{
				_selectedScheduleType = value;
				OnPropertyChanged(() => SelectedScheduleType);
				IsAccessSchedule = value == GKScheduleType.Access;
			}
		}

		public ObservableCollection<GKSchedulePeriodType> SchedulePeriodTypes { get; private set; }

		GKSchedulePeriodType _selectedSchedulePeriodType;
		public GKSchedulePeriodType SelectedSchedulePeriodType
		{
			get { return _selectedSchedulePeriodType; }
			set
			{
				_selectedSchedulePeriodType = value;
				OnPropertyChanged(() => SelectedSchedulePeriodType);
				ShowHoursPeriod = value == GKSchedulePeriodType.Custom;
			}
		}

		bool _isAccessSchedule;
		public bool IsAccessSchedule
		{
			get { return _isAccessSchedule; }
			set
			{
				_isAccessSchedule = value;
				OnPropertyChanged(() => IsAccessSchedule);
			}
		}

		bool _showHoursPeriod;
		public bool ShowHoursPeriod
		{
			get { return _showHoursPeriod; }
			set
			{
				_showHoursPeriod = value;
				OnPropertyChanged(() => ShowHoursPeriod);
			}
		}

		public ObservableCollection<GKSchedule> Holidays { get; private set; }

		GKSchedule _selectedHoliday;
		public GKSchedule SelectedHoliday
		{
			get { return _selectedHoliday; }
			set
			{
				_selectedHoliday = value;
				OnPropertyChanged(() => SelectedHoliday);
			}
		}

		public ObservableCollection<GKSchedule> WorkHolidays { get; private set; }

		GKSchedule _selectedWorkHoliday;
		public GKSchedule SelectedWorkHoliday
		{
			get { return _selectedWorkHoliday; }
			set
			{
				_selectedWorkHoliday = value;
				OnPropertyChanged(() => SelectedWorkHoliday);
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

			Schedule.No = No;
			Schedule.Name = Name;
			Schedule.Description = Description;
			Schedule.StartDateTime = StartDateTime;
			Schedule.ScheduleType = SelectedScheduleType;
			Schedule.SchedulePeriodType = SelectedSchedulePeriodType;
			Schedule.HoursPeriod = HoursPeriod;

			//var result = FiresecManager.FiresecService.GKSetSchedule(Schedule);
			//if (result.HasError)
			//{
			//    MessageBoxService.ShowError(result.Error);
			//}

			return base.Save();
		}
	}
}