using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using FiresecAPI.GK;

namespace GKModule.ViewModels
{
	public class SchedulesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public SchedulesViewModel()
		{
			Menu = new SchedulesMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Schedules = new ObservableCollection<ScheduleViewModel>();
			foreach (var schedule in GKManager.DeviceConfiguration.Schedules.OrderBy(x => x.No))
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
			}
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
						var daySchedule = GKManager.DeviceConfiguration.DaySchedules.FirstOrDefault();
						if (daySchedule != null)
						{
							schedule.DayScheduleUIDs.Add(daySchedule.UID);
						}
					}
				}
				GKManager.DeviceConfiguration.Schedules.Add(schedule);
				var scheduleViewModel = new ScheduleViewModel(schedule);
				Schedules.Add(scheduleViewModel);
				SelectedSchedule = scheduleViewModel;
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить график работ " + SelectedSchedule.Schedule.PresentationName))
			{
				var index = Schedules.IndexOf(SelectedSchedule);
				GKManager.DeviceConfiguration.Schedules.Remove(SelectedSchedule.Schedule);
				SelectedSchedule.Schedule.OnChanged();
				Schedules.Remove(SelectedSchedule);
				index = Math.Min(index, Schedules.Count - 1);
				if (index > -1)
					SelectedSchedule = Schedules[index];
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var scheduleDetailsViewModel = new ScheduleDetailsViewModel(SelectedSchedule.Schedule);
			if (DialogService.ShowModalWindow(scheduleDetailsViewModel))
			{
				SelectedSchedule.Update(scheduleDetailsViewModel.Schedule);
				scheduleDetailsViewModel.Schedule.OnChanged();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public override void OnShow()
		{
			base.OnShow();
			SelectedSchedule = SelectedSchedule;
		}

		#region ISelectable<Guid> Members

		public void Select(Guid scheduleUID)
		{
			if (scheduleUID != Guid.Empty)
				SelectedSchedule = Schedules.FirstOrDefault(x => x.Schedule.UID == scheduleUID);
		}

		#endregion

		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}

		void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
					new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", AddCommand, "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "/Controls;component/Images/BDelete.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 2 }
			};
		}
	}
}