using System;
using System.Linq;
using FiresecAPI.GK;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using FiresecClient;
using Common;
using System.Collections.ObjectModel;

namespace GKModule.ViewModels
{
	public class ScheduleViewModel : BaseViewModel
	{
		public GKSchedule Schedule { get; set; }
		public CalendarViewModel Calendar { get; private set; }

		public ScheduleViewModel(GKSchedule schedule)
		{
			Calendar = new CalendarViewModel(schedule.Calendar);
			WriteCommand = new RelayCommand(OnWrite);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);

			Schedule = schedule;
			Update();
		}

		public string Name
		{
			get { return Schedule.Name; }
			set
			{
				Schedule.Name = value;
				Schedule.OnChanged();
				OnPropertyChanged(() => Name);
			}
		}

		public string Description
		{
			get { return Schedule.Description; }
			set
			{
				Schedule.Description = value;
				Schedule.OnChanged();
				OnPropertyChanged(() => Description);
			}
		}

		public void Update(GKSchedule schedule)
		{
			Schedule = schedule;
			OnPropertyChanged(() => Schedule);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
			Update();
		}
		public void Update()
		{
			Parts = new SortableObservableCollection<SchedulePartViewModel>();
			for (int i = 0; i < Schedule.DayScheduleUIDs.Count; i++)
			{
				var dayScheduleUID = Schedule.DayScheduleUIDs[i];
				var daySchedule = GKManager.DeviceConfiguration.DaySchedules.FirstOrDefault(x => x.UID == dayScheduleUID);
				var schedulePartViewModel = new SchedulePartViewModel(Schedule, dayScheduleUID, i);
				Parts.Add(schedulePartViewModel);
			}
			SelectedPart = Parts.FirstOrDefault();
		}

		public RelayCommand WriteCommand { get; private set; }
		void OnWrite()
		{
			var result = FiresecManager.FiresecService.GKSetSchedule(Schedule);
			if (result.HasError)
			{
				MessageBoxService.ShowError(result.Error);
			}
		}

		ObservableCollection<SchedulePartViewModel> _parts;
		public ObservableCollection<SchedulePartViewModel> Parts
		{
			get { return _parts; }
			set
			{
				_parts = value;
				OnPropertyChanged(() => Parts);
			}
		}

		SchedulePartViewModel _selectedPart;
		public SchedulePartViewModel SelectedPart
		{
			get { return _selectedPart; }
			set
			{
				_selectedPart = value;
				OnPropertyChanged(() => SelectedPart);
			}
		}

		DateTime _selectedHoliday;
		public DateTime SelectedHoliday
		{
			get { return _selectedHoliday; }
			set
			{
				_selectedHoliday = value;
				if (Schedule != null)
				{
					if (Schedule.Holidays.Contains(_selectedHoliday))
						Schedule.Holidays.Remove(_selectedHoliday);
					else
						Schedule.Holidays.Add(_selectedHoliday);
				}
				OnPropertyChanged(() => SelectedHoliday);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var daysCount = 1;
			if (Schedule.SchedulePeriodType == GKSchedulePeriodType.Weekly)
			{
				daysCount = 7;
			}
			for (int i = 0; i < daysCount; i++)
			{
				var daySchedule = GKManager.DeviceConfiguration.DaySchedules.FirstOrDefault();
				if (daySchedule != null)
				{
					Schedule.DayScheduleUIDs.Add(daySchedule.UID);
					var schedulePartViewModel = new SchedulePartViewModel(Schedule, Guid.Empty, Schedule.DayScheduleUIDs.Count - 1);
					Parts.Add(schedulePartViewModel);
				}
			}
			SelectedPart = Parts.LastOrDefault();
		}
		bool CanAdd()
		{
			return Parts.Count < 50;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (Schedule.SchedulePeriodType == GKSchedulePeriodType.Weekly)
			{
				var weekNo = SelectedPart.Index / 7;
				for (int i = 6; i >= 0; i--)
				{
					var index = weekNo * 7 + i;
					Schedule.DayScheduleUIDs.RemoveAt(index);
					Parts.RemoveAt(index);
				}
			}
			else
			{
				Schedule.DayScheduleUIDs.Remove(SelectedPart.SelectedDaySchedule.UID);
				Parts.Remove(SelectedPart);
			}
			Update();
		}
		bool CanDelete()
		{
			if (SelectedPart == null)
				return false;
			if (Schedule.SchedulePeriodType == GKSchedulePeriodType.Dayly)
				return Parts.Count > 1;
			return Parts.Count > 7;
		}
	}
}