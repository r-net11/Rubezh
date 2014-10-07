using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using GKModule.Plans;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using KeyboardKey = System.Windows.Input.Key;

namespace GKModule.ViewModels
{
	public class SchedulesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		bool _lockSelection;
		public static SchedulesViewModel Current { get; private set; }

		public SchedulesViewModel()
		{
			Menu = new SchedulesMenuViewModel(this);
			Current = this;
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			RegisterShortcuts();
			SubscribeEvents();
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
			OnAddResult();
		}
		ScheduleDetailsViewModel OnAddResult()
		{
			var scheduleDetailsViewModel = new ScheduleDetailsViewModel();
			if (DialogService.ShowModalWindow(scheduleDetailsViewModel))
			{
				GKManager.DeviceConfiguration.Schedules.Add(scheduleDetailsViewModel.Schedule);
				var scheduleViewModel = new ScheduleViewModel(scheduleDetailsViewModel.Schedule);
				Schedules.Add(scheduleViewModel);
				SelectedSchedule = scheduleViewModel;
				ServiceFactory.SaveService.GKChanged = true;
				GKPlanExtension.Instance.Cache.BuildSafe<GKSchedule>();
				return scheduleDetailsViewModel;
			}
			return null;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestionYesNo("Вы уверены, что хотите удалить график работ " + SelectedSchedule.Schedule.PresentationName))
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
			OnEdit(SelectedSchedule.Schedule);
		}
		void OnEdit(GKSchedule schedule)
		{
			var scheduleDetailsViewModel = new ScheduleDetailsViewModel(schedule);
			if (DialogService.ShowModalWindow(scheduleDetailsViewModel))
			{
				SelectedSchedule.Update(scheduleDetailsViewModel.Schedule);
				scheduleDetailsViewModel.Schedule.OnChanged();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public void CreateSchedule(CreateXScheduleEventArg createScheduleEventArg)
		{
			ScheduleDetailsViewModel result = OnAddResult();
			if (result == null)
			{
				createScheduleEventArg.Cancel = true;
				createScheduleEventArg.ScheduleUID = Guid.Empty;
			}
			else
			{
				createScheduleEventArg.Cancel = false;
				createScheduleEventArg.ScheduleUID = result.Schedule.UID;
				createScheduleEventArg.Schedule = result.Schedule;
			}
		}
		public void EditSchedule(Guid scheduleUID)
		{
			var scheduleViewModel = scheduleUID == Guid.Empty ? null : Schedules.FirstOrDefault(x => x.Schedule.UID == scheduleUID);
			if (scheduleViewModel != null)
				OnEdit(scheduleViewModel.Schedule);
		}

		public override void OnShow()
		{
			base.OnShow();
			SelectedSchedule = SelectedSchedule;
		}

		public override void OnHide()
		{
			base.OnHide();
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
		public void LockedSelect(Guid scheduleUID)
		{
			_lockSelection = true;
			Select(scheduleUID);
			_lockSelection = false;
		}

		void SubscribeEvents()
		{
		}
		void OnScheduleChanged(Guid scheduleUID)
		{
			var schedule = Schedules.FirstOrDefault(x => x.Schedule.UID == scheduleUID);
			if (schedule != null)
			{
				schedule.Update();
				if (!_lockSelection)
					SelectedSchedule = schedule;
			}
		}

		private void SetRibbonItems()
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