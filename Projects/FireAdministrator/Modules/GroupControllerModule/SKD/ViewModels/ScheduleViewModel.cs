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
			ReadCommand = new RelayCommand(OnRead);
			WriteCommand = new RelayCommand(OnWrite);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);

			Schedule = schedule;
			Update();

			Parts = new SortableObservableCollection<SchedulePartViewModel>();
			for (int i = 0; i < Schedule.DayIntervalUIDs.Count; i++)
			{
				var dayIntervalPartUID = Schedule.DayIntervalUIDs[i];
				var schedulePartViewModel = new SchedulePartViewModel(Schedule, dayIntervalPartUID, i);
				Parts.Add(schedulePartViewModel);
			}
			SelectedPart = Parts.FirstOrDefault();
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

		public RelayCommand ReadCommand { get; private set; }
		void OnRead()
		{
			var result = FiresecManager.FiresecService.GKGetSchedule(Schedule.No);
			if (!result.HasError && result.Result != null)
			{
				var schedule = result.Result;
			}
			else
			{
				MessageBoxService.ShowError(result.Error);
			}
			ServiceFactory.SaveService.GKChanged = true;
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

		public SortableObservableCollection<SchedulePartViewModel> Parts { get; private set; }

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
			Schedule.DayIntervalUIDs.Add(Guid.Empty);
			var schedulePartViewModel = new SchedulePartViewModel(Schedule, Guid.Empty, Schedule.DayIntervalUIDs.Count - 1);
			Parts.Add(schedulePartViewModel);
			SelectedPart = schedulePartViewModel;
			ServiceFactory.SaveService.GKChanged = true;
		}
		bool CanAdd()
		{
			return true;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			Schedule.DayIntervalUIDs.Remove(SelectedPart.SelectedDaySchedule.UID);
			Parts.Remove(SelectedPart);
			ServiceFactory.SaveService.GKChanged = true;
		}
		bool CanDelete()
		{
			return SelectedPart != null && Parts.Count > 1;
		}
	}
}