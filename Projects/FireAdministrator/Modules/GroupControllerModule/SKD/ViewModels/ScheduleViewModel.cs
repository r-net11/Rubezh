using System;
using System.Linq;
using System.Collections.Generic;
using FiresecAPI.GK;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using FiresecClient;
using Common;

namespace GKModule.ViewModels
{
	public class ScheduleViewModel : BaseViewModel
	{
		public GKSchedule Schedule { get; set; }

		public ScheduleViewModel(GKSchedule schedule)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);

			Schedule = schedule;
			Update();


			DayIntervalParts = new SortableObservableCollection<ScheduleIntervalPartViewModel>();
			foreach (var dayIntervalPart in Schedule.DayIntervalParts)
			{
				var dayIntervalPartViewModel = new ScheduleIntervalPartViewModel(dayIntervalPart);
				DayIntervalParts.Add(dayIntervalPartViewModel);
			}
			SelectedDayIntervalPart = DayIntervalParts.FirstOrDefault();
		}

		public string Name
		{
			get { return Schedule.Name; }
			set
			{
				Schedule.Name = value;
				Schedule.OnChanged();
				OnPropertyChanged(() => Name);
				ServiceFactory.SaveService.GKChanged = true;
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
				ServiceFactory.SaveService.GKChanged = true;
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
		}

		public SortableObservableCollection<ScheduleIntervalPartViewModel> DayIntervalParts { get; private set; }

		ScheduleIntervalPartViewModel _selectedDayIntervalPart;
		public ScheduleIntervalPartViewModel SelectedDayIntervalPart
		{
			get { return _selectedDayIntervalPart; }
			set
			{
				_selectedDayIntervalPart = value;
				OnPropertyChanged(() => SelectedDayIntervalPart);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var scheduleIntervalPartDetailsViewModel = new ScheduleIntervalPartDetailsViewModel(Schedule);
			if (DialogService.ShowModalWindow(scheduleIntervalPartDetailsViewModel))
			{
				var gkIntervalPart = scheduleIntervalPartDetailsViewModel.GKIntervalPart;
				Schedule.DayIntervalParts.Add(gkIntervalPart);
				var scheduleIntervalPartViewModel = new ScheduleIntervalPartViewModel(gkIntervalPart);
				DayIntervalParts.Add(scheduleIntervalPartViewModel);
				DayIntervalParts.Sort(item => item.BeginTime);
				SelectedDayIntervalPart = scheduleIntervalPartViewModel;
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanAdd()
		{
			return true;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			Schedule.DayIntervalParts.Remove(SelectedDayIntervalPart.IntervalPart);
			DayIntervalParts.Remove(SelectedDayIntervalPart);
			ServiceFactory.SaveService.GKChanged = true;
		}
		bool CanDelete()
		{
			return SelectedDayIntervalPart != null && DayIntervalParts.Count > 1;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var dayIntervalPartDetailsViewModel = new ScheduleIntervalPartDetailsViewModel(Schedule, SelectedDayIntervalPart.IntervalPart);
			if (DialogService.ShowModalWindow(dayIntervalPartDetailsViewModel))
			{
				SelectedDayIntervalPart.Update();
				var selectedDayIntervalPart = SelectedDayIntervalPart;
				DayIntervalParts.Sort(item => item.BeginTime);
				SelectedDayIntervalPart = selectedDayIntervalPart;
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanEdit()
		{
			return SelectedDayIntervalPart != null;
		}
	}
}