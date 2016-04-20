using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.ViewModels;
using Common;
using RubezhClient;
using KeyboardKey = System.Windows.Input.Key;
using System.Windows.Input;

namespace AutomationModule.ViewModels
{
	public class SchedulesViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public SchedulesViewModel()
		{
			Menu = new SchedulesMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			RegisterShortcuts();
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
		}

		public void Initialize()
		{
			Schedules = new SortableObservableCollection<ScheduleViewModel>();
			if (ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSchedules == null)
				ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSchedules = new List<AutomationSchedule>();
			foreach (var schedule in ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSchedules)
			{
				var scheduleViewModel = new ScheduleViewModel(schedule);
				Schedules.Add(scheduleViewModel);
			}
			SelectedSchedule = Schedules.FirstOrDefault();
			OnPropertyChanged(()=>Schedules);
		}


		public SortableObservableCollection<ScheduleViewModel> Schedules { get; private set; }

		ScheduleViewModel _selectedSchedule;
		public ScheduleViewModel SelectedSchedule
		{
			get { return _selectedSchedule; }
			set
			{
				_selectedSchedule = value;
				OnPropertyChanged(() => SelectedSchedule);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var scheduleDetailsViewModel = new ScheduleDetailsViewModel();
			if (DialogService.ShowModalWindow(scheduleDetailsViewModel))
			{
				ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSchedules.Add(scheduleDetailsViewModel.Schedule);
				var scheduleViewModel = new ScheduleViewModel(scheduleDetailsViewModel.Schedule);
				Schedules.Add(scheduleViewModel);
				SelectedSchedule = scheduleViewModel;
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		bool CanAdd()
		{
			return true;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var index = Schedules.IndexOf(SelectedSchedule);
			ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSchedules.Remove(SelectedSchedule.Schedule);
			Schedules.Remove(SelectedSchedule);
			index = Math.Min(index, Schedules.Count - 1);
			if (index > -1)
				SelectedSchedule = Schedules[index];
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

		public override void OnShow()
		{
			if (SelectedSchedule != null)
				SelectedSchedule.UpdateContent();

			if (Schedules != null)
				Schedules.Sort(x => x.Name);

			base.OnShow();
		}
	}
}