using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI.Models;
using FiresecAPI.GK;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using GKModule.Events;
using GKModule.Plans;
using KeyboardKey = System.Windows.Input.Key;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class DaySchedulesViewModel : ViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public DaySchedulesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditRemove);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
			RegisterShortcuts();
		}

		public void Initialize()
		{
			DaySchedules = new ObservableCollection<DayScheduleViewModel>();
			foreach (var dayInterval in GKManager.DeviceConfiguration.DaySchedules)
			{
				var dayScheduleViewModel = new DayScheduleViewModel(dayInterval);
				DaySchedules.Add(dayScheduleViewModel);
			}
			SelectedDaySchedule = DaySchedules.FirstOrDefault();
		}

		ObservableCollection<DayScheduleViewModel> _daySchedules;
		public ObservableCollection<DayScheduleViewModel> DaySchedules
		{
			get { return _daySchedules; }
			set
			{
				_daySchedules = value;
				OnPropertyChanged(() => DaySchedules);
			}
		}

		DayScheduleViewModel _selectedDaySchedule;
		public DayScheduleViewModel SelectedDaySchedule
		{
			get { return _selectedDaySchedule; }
			set
			{
				_selectedDaySchedule = value;
				OnPropertyChanged(() => SelectedDaySchedule);
			}
		}

		public void Select(Guid dayIntervalUID)
		{
			if (dayIntervalUID != Guid.Empty)
			{
				var dayIntervalViewModel = DaySchedules.FirstOrDefault(x => x.DaySchedule.UID == dayIntervalUID);
				SelectedDaySchedule = dayIntervalViewModel;
			}
		}

		bool CanEditRemove()
		{
			return SelectedDaySchedule != null && SelectedDaySchedule.IsEnabled;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var dayScheduleDetailsViewModel = new DayScheduleDetailsViewModel();
			if (DialogService.ShowModalWindow(dayScheduleDetailsViewModel))
			{
				GKManager.DeviceConfiguration.DaySchedules.Add(dayScheduleDetailsViewModel.DaySchedule);
				var dayScheduleViewModel = new DayScheduleViewModel(dayScheduleDetailsViewModel.DaySchedule);
				DaySchedules.Add(dayScheduleViewModel);
				SelectedDaySchedule = dayScheduleViewModel;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (SelectedDaySchedule.ConfirmDeactivation())
			{
				var index = DaySchedules.IndexOf(SelectedDaySchedule);
				GKManager.DeviceConfiguration.DaySchedules.Remove(SelectedDaySchedule.DaySchedule);
				DaySchedules.Remove(SelectedDaySchedule);
				index = Math.Min(index, DaySchedules.Count - 1);
				if (index > -1)
					SelectedDaySchedule = DaySchedules[index];
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var dayScheduleDetailsViewModel = new DayScheduleDetailsViewModel(SelectedDaySchedule.DaySchedule);
			if (DialogService.ShowModalWindow(dayScheduleDetailsViewModel))
			{
				SelectedDaySchedule.Update(dayScheduleDetailsViewModel.DaySchedule);
			}
		}

		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), () =>
			{
				if (SelectedDaySchedule != null)
				{
					if (AddCommand.CanExecute(null))
						AddCommand.Execute();
				}
			});
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), () =>
			{
				if (SelectedDaySchedule != null)
				{
					if (DeleteCommand.CanExecute(null))
						DeleteCommand.Execute();
				}
			});
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), () =>
			{
				if (SelectedDaySchedule != null)
				{
					if (EditCommand.CanExecute(null))
						EditCommand.Execute();
				}
			});
		}
	}
}