using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using RubezhAPI.GK;
using RubezhClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace GKModule.ViewModels
{
	public class SchedulesViewModel : ViewPartViewModel
	{
		public SchedulesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			RegisterShortcuts();
		}

		public void Initialize()
		{
			Schedules = new ObservableCollection<ScheduleViewModel>();
			var schedules = GKScheduleHelper.GetSchedules();
			if (schedules != null)
			{
				foreach (var schedule in schedules.OrderBy(x => x.No))
				{
					var scheduleViewModel = new ScheduleViewModel(schedule);
					Schedules.Add(scheduleViewModel);
				}
				SelectedSchedule = Schedules.FirstOrDefault();
			}
		}

		ObservableCollection<ScheduleViewModel> _schedules;
		public ObservableCollection<ScheduleViewModel> Schedules
		{
			get { return _schedules; }
			set
			{
				_schedules = value;
				OnPropertyChanged(() => Schedules);
			}
		}

		ScheduleViewModel _selectedSchedule;
		public ScheduleViewModel SelectedSchedule
		{
			get { return _selectedSchedule; }
			set
			{
				_selectedSchedule = value;
				if (value != null)
				{
					value.Update();
				}
				OnPropertyChanged(() => SelectedSchedule);
				OnPropertyChanged(() => HasSelectedSchedule);
				OnPropertyChanged(() => IsHolidaySchedule);
			}
		}

		public bool IsHolidaySchedule
		{
			get { return (SelectedSchedule != null && SelectedSchedule.Schedule != null && SelectedSchedule.Schedule.ScheduleType != GKScheduleType.Access); }
		}

		public bool HasSelectedSchedule
		{
			get { return SelectedSchedule != null; }
		}

		bool CanEditDelete()
		{
			return SelectedSchedule != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var scheduleDetailsViewModel = new ScheduleDetailsViewModel();
			if (DialogService.ShowModalWindow(scheduleDetailsViewModel))
			{
				var schedule = scheduleDetailsViewModel.Schedule;
				if (schedule.SchedulePeriodType == GKSchedulePeriodType.Weekly)
				{
					for (int i = 0; i < 7; i++)
					{
						var daySchedule = GKModuleLoader.DaySchedulesViewModel.GetDaySchedules().FirstOrDefault();
						if (daySchedule != null)
						{
							schedule.ScheduleParts.Add(new GKSchedulePart() { DayNo = i, DayScheduleUID = daySchedule.UID });
						}
					}
				}
				var saveScheduleResult = GKScheduleHelper.SaveSchedule(schedule, true);
				if (saveScheduleResult)
				{
					var scheduleViewModel = new ScheduleViewModel(schedule);
					Schedules.Add(scheduleViewModel);
					SelectedSchedule = scheduleViewModel;
				}
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить график работ " + SelectedSchedule.Schedule.PresentationName))
			{
				var index = Schedules.IndexOf(SelectedSchedule);
				if (GKScheduleHelper.DeleteSchedule(SelectedSchedule.Schedule))
				{
					Schedules.Remove(SelectedSchedule);
					index = Math.Min(index, Schedules.Count - 1);
					if (index > -1)
						SelectedSchedule = Schedules[index];
				}
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var scheduleDetailsViewModel = new ScheduleDetailsViewModel(SelectedSchedule.Schedule);
			if (DialogService.ShowModalWindow(scheduleDetailsViewModel))
			{
				var schedule = scheduleDetailsViewModel.Schedule;
				if (GKScheduleHelper.SaveSchedule(schedule, false))
				{
					SelectedSchedule.Update(schedule);
					scheduleDetailsViewModel.Schedule.OnChanged();
				}
			}
		}

		public override void OnShow()
		{
			base.OnShow();
			SelectedSchedule = SelectedSchedule;
		}

		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}
	}
}