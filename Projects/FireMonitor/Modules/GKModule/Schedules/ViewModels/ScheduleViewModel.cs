using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.GK;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class ScheduleViewModel : BaseViewModel
	{
		public GKSchedule Schedule { get; set; }
		public CalendarViewModel Calendar { get; private set; }
		List<GKDaySchedule> _DaySchedules { get{ return GKModuleLoader.DaySchedulesViewModel.GetDaySchedules(); }}

		public ScheduleViewModel(GKSchedule schedule)
		{
			Calendar = new CalendarViewModel(schedule);
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
			for (int i = 0; i < Schedule.ScheduleParts.Count; i++)
			{
				var dayScheduleUID = Schedule.ScheduleParts[i].DayScheduleUID;
				var daySchedule = _DaySchedules.FirstOrDefault(x => x.UID == dayScheduleUID);
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
				var daySchedule = _DaySchedules.FirstOrDefault();
				if (daySchedule != null)
				{
					Schedule.ScheduleParts.Add(new GKSchedulePart() { DayNo = i, DayScheduleUID = daySchedule.UID });
					if (UpdateSchedule())
					{
						var schedulePartViewModel = new SchedulePartViewModel(Schedule, Guid.Empty, i);
						Parts.Add(schedulePartViewModel);
					}
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
					if (UpdateSchedule())
					{
						Schedule.ScheduleParts.RemoveAt(index);
					}
					Parts.RemoveAt(index);
				}
			}
			else
			{
				Schedule.ScheduleParts.RemoveAll(x => x.DayScheduleUID == SelectedPart.SelectedDaySchedule.UID);
				if (UpdateSchedule())
				{
					Parts.Remove(SelectedPart);
				}
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

		public bool UpdateSchedule()
		{
			return GKScheduleHelper.SaveSchedule(Schedule, false);
		}
	}
}