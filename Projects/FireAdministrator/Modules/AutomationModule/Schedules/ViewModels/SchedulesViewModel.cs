using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;

namespace AutomationModule.ViewModels
{
	public class SchedulesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public SchedulesViewModel()
		{
			Menu = new SchedulesMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
		}

		public void Initialize()
		{
			Schedules = new ObservableCollection<ScheduleViewModel>();
			if (FiresecClient.FiresecManager.SystemConfiguration.AutomationSchedules == null)
				FiresecClient.FiresecManager.SystemConfiguration.AutomationSchedules = new List<AutomationSchedule>();
			foreach (var schedule in FiresecClient.FiresecManager.SystemConfiguration.AutomationSchedules)
			{
				var scheduleViewModel = new ScheduleViewModel(schedule);
				Schedules.Add(scheduleViewModel);
			}
			SelectedSchedule = Schedules.FirstOrDefault();
		}

		ObservableCollection<ScheduleViewModel> _schedules;
		public ObservableCollection<ScheduleViewModel> Schedules
		{
			get { return _schedules; }
			set
			{
				_schedules = value;
				OnPropertyChanged("Schedules");
			}
		}

		ScheduleViewModel _selectedSchedule;
		public ScheduleViewModel SelectedSchedule
		{
			get { return _selectedSchedule; }
			set
			{
				_selectedSchedule = value;
				OnPropertyChanged("SelectedSchedule");
			}
		}

		bool _isNowPlaying;
		public bool IsNowPlaying
		{
			get { return _isNowPlaying; }
			set
			{
				_isNowPlaying = value;
				OnPropertyChanged("IsNowPlaying");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var scheduleDetailsViewModel = new ScheduleDetailsViewModel();
			if (DialogService.ShowModalWindow(scheduleDetailsViewModel))
			{
				FiresecClient.FiresecManager.SystemConfiguration.AutomationSchedules.Add(scheduleDetailsViewModel.Schedule);
				ServiceFactory.SaveService.AutomationChanged = true;
				var scheduleViewModel = new ScheduleViewModel(scheduleDetailsViewModel.Schedule);
				Schedules.Add(scheduleViewModel);
				SelectedSchedule = scheduleViewModel;
			}
		}

		bool CanAdd()
		{
			return true;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			FiresecClient.FiresecManager.SystemConfiguration.AutomationSchedules.Remove(SelectedSchedule.Schedule);
			Schedules.Remove(SelectedSchedule);
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var scheduleDetailsViewModel = new ScheduleDetailsViewModel(SelectedSchedule.Schedule);
			if (DialogService.ShowModalWindow(scheduleDetailsViewModel))
			{
				SelectedSchedule.Schedule = scheduleDetailsViewModel.Schedule;
				SelectedSchedule.Update();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		bool CanEditDelete()
		{
			return SelectedSchedule != null;
		}

		public void Select(Guid scheduleUid)
		{
			if (scheduleUid != Guid.Empty)
			{
				SelectedSchedule = Schedules.FirstOrDefault(item => item.Schedule.Uid == scheduleUid);
			}
		}
	}
}
