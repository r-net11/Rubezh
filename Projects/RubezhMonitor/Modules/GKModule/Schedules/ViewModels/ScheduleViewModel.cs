using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using RubezhAPI.GK;
using RubezhClient;
using RubezhClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class ScheduleViewModel : BaseViewModel
	{
		public GKSchedule Schedule { get; set; }
		public CalendarViewModel Calendar { get; private set; }
		List<GKDaySchedule> DaySchedules { get{ return GKModuleLoader.DaySchedulesViewModel.GetDaySchedules(); }}

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
			foreach (var schedulePart in Schedule.ScheduleParts.OrderBy(x => x.DayNo))
			{
				var schedulePartViewModel = new SchedulePartViewModel(Schedule, schedulePart);
				Parts.Add(schedulePartViewModel);
			}
			SelectedPart = Parts.FirstOrDefault();
		}

		public RelayCommand WriteCommand { get; private set; }
		void OnWrite()
		{
			var result = ClientManager.RubezhService.GKSetSchedule(Schedule);
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
			var daySchedule = DaySchedules.FirstOrDefault();
			if (daySchedule != null)
			{
				var daysCount = Schedule.SchedulePeriodType == GKSchedulePeriodType.Weekly ? 7 : 1;
				for (int i = 0; i < daysCount; i++)
				{
					Schedule.ScheduleParts.Add(new GKSchedulePart() { DayNo = Schedule.ScheduleParts.Count, DayScheduleUID = daySchedule.UID });
				}
				if (GKScheduleHelper.SaveSchedule(Schedule, false))
				{
					Update();
				}
			}
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
				var weekNo = SelectedPart.DayNo / 7;
				for (int i = 6; i >= 0; i--)
				{
					var index = weekNo * 7 + i;
					Schedule.ScheduleParts.RemoveAt(index);
				}
			}
			else
			{
				Schedule.ScheduleParts.RemoveAll(x => x.DayNo == SelectedPart.DayNo);
			}
			for (int i = 0; i < Schedule.ScheduleParts.Count; i++)
			{
				Schedule.ScheduleParts[i].DayNo = i;
			}
			if (GKScheduleHelper.SaveSchedule(Schedule, false))
			{
				Update();
			}
		}
		bool CanDelete()
		{
			return SelectedPart != null;

			//if (SelectedPart == null)
			//	return false;
			//if (Schedule.SchedulePeriodType == GKSchedulePeriodType.Dayly)
			//	return Parts.Count > 1;
			//return Parts.Count > 7;
		}
	}
}