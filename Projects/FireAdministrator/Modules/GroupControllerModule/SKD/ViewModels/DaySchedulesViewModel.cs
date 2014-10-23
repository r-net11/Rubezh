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
using Infrastructure.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using GKModule.Events;
using GKModule.Plans;
using KeyboardKey = System.Windows.Input.Key;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class DaySchedulesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public DaySchedulesViewModel()
		{
			Menu = new DaySchedulesMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditRemove);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
			RegisterShortcuts();
			IsRightPanelEnabled = true;
			SetRibbonItems();
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
				UpdateRibbonItems();
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
				ServiceFactory.SaveService.GKChanged = true;
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
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var dayScheduleDetailsViewModel = new DayScheduleDetailsViewModel(SelectedDaySchedule.DaySchedule);
			if (DialogService.ShowModalWindow(dayScheduleDetailsViewModel))
			{
				SelectedDaySchedule.Update(dayScheduleDetailsViewModel.DaySchedule);
				ServiceFactory.SaveService.GKChanged = true;
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

		protected override void UpdateRibbonItems()
		{
			base.UpdateRibbonItems();
			RibbonItems[0][0].Command = AddCommand;
			RibbonItems[0][1].Command = SelectedDaySchedule == null ? null : EditCommand;
			RibbonItems[0][2].Command = SelectedDaySchedule == null ? null : DeleteCommand;
		}
		void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Редактировать", "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("Удалить", "/Controls;component/Images/BDelete.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 1 }
			};
		}
	}
}