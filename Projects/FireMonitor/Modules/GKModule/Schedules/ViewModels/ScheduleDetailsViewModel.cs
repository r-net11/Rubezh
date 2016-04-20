using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient.SKDHelpers;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class ScheduleDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKSchedule Schedule;
		public bool IsNew { get; private set; }
		List<GKSchedule> _schedules;
		public ScheduleDetailsViewModel(GKSchedule schedule = null)
		{
			_schedules = GKScheduleHelper.GetSchedules();
			if (_schedules == null)
				_schedules = new List<GKSchedule>();
			if (schedule == null)
			{
				Title = "Создание нового графика доступа";
				IsNew = true;

				Schedule = new GKSchedule()
				{
					Name = "Новый график доступа",
					No = 1
				};
				if (_schedules.Count != 0)
					Schedule.No = (ushort)(_schedules.Select(x => x.No).Max() + 1);
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
			foreach (var holidaySchedule in _schedules)
			{
				if (holidaySchedule.ScheduleType == GKScheduleType.Holiday)
				{
					Holidays.Add(holidaySchedule);
				}
			}
			SelectedHoliday = Holidays.FirstOrDefault(x => x.No == Schedule.HolidayScheduleNo);

			WorkHolidays = new ObservableCollection<GKSchedule>();
			foreach (var workHolidaySchedule in _schedules)
			{
				if (workHolidaySchedule.ScheduleType == GKScheduleType.WorkHoliday)
				{
					WorkHolidays.Add(workHolidaySchedule);
				}
			}
			SelectedWorkHoliday = WorkHolidays.FirstOrDefault(x => x.No == Schedule.WorkHolidayScheduleNo);

			CopyProperties();

			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingSchedule in _schedules)
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

		public ObservableCollection<GKScheduleType> ScheduleTypes { get; private set; }

		GKScheduleType _selectedScheduleType;
		public GKScheduleType SelectedScheduleType
		{
			get { return _selectedScheduleType; }
			set
			{
				_selectedScheduleType = value;
				OnPropertyChanged(() => SelectedScheduleType);
				OnPropertyChanged(() => IsAccessSchedule);
				OnPropertyChanged(() => CanSetStartDateTime);
				OnPropertyChanged(() => ShowHoursPeriod);
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
				OnPropertyChanged(() => CanSetStartDateTime);
				OnPropertyChanged(() => ShowHoursPeriod);
			}
		}

		public bool IsAccessSchedule
		{
			get { return SelectedScheduleType == GKScheduleType.Access; }
		}

		public bool CanSetStartDateTime
		{
			get { return SelectedScheduleType == GKScheduleType.Access && (SelectedSchedulePeriodType == GKSchedulePeriodType.Custom || SelectedSchedulePeriodType == GKSchedulePeriodType.NonPeriodic); }
		}

		public bool ShowHoursPeriod
		{
			get { return SelectedScheduleType == GKScheduleType.Access && SelectedSchedulePeriodType == GKSchedulePeriodType.Custom; }
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
			if (No < 1 || No > 255)
			{
				MessageBoxService.Show("Номер должен быть задан в диапазоне от 1 до 255");
				return false;
			}
			if (Schedule.No != No && _schedules.Any(x => x.No == No))
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
			if (SelectedHoliday != null)
				Schedule.HolidayScheduleNo = SelectedHoliday.No;
			if (SelectedWorkHoliday != null)
				Schedule.WorkHolidayScheduleNo = SelectedWorkHoliday.No;

			return base.Save();
		}
	}
}