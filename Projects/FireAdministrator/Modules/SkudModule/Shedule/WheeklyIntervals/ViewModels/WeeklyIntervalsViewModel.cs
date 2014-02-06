using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace SKDModule.ViewModels
{
	public class WeeklyIntervalsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public WeeklyIntervalsViewModel()
		{
			Menu = new WeeklyIntervalsMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			RegisterShortcuts();
			SetRibbonItems();

			WeeklyIntervals = new ObservableCollection<WeeklyIntervalViewModel>();
			foreach (var weeklyInterval in SKDManager.SKDConfiguration.WeeklyIntervals)
			{
				var timeInrervalViewModel = new WeeklyIntervalViewModel(weeklyInterval);
				WeeklyIntervals.Add(timeInrervalViewModel);
			}
			SelectedWeeklyInterval = WeeklyIntervals.FirstOrDefault();
		}

		SKDWeeklyInterval IntervalToCopy;

		public ObservableCollection<WeeklyIntervalViewModel> WeeklyIntervals { get; private set; }

		WeeklyIntervalViewModel _selectedWeeklyInterval;
		public WeeklyIntervalViewModel SelectedWeeklyInterval
		{
			get { return _selectedWeeklyInterval; }
			set
			{
				_selectedWeeklyInterval = value;
				OnPropertyChanged("SelectedWeeklyInterval");
			}
		}

		public void Select(Guid intervalUID)
		{
			if (intervalUID != Guid.Empty)
			{
				var intervalViewModel = WeeklyIntervals.FirstOrDefault(x => x.WeeklyInterval.UID == intervalUID);
				if (intervalViewModel != null)
				{
					SelectedWeeklyInterval = intervalViewModel;
				}
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var weeklyIntervalDetailsViewModel = new WeeklyIntervalDetailsViewModel();
			if (DialogService.ShowModalWindow(weeklyIntervalDetailsViewModel))
			{
				SKDManager.SKDConfiguration.WeeklyIntervals.Add(weeklyIntervalDetailsViewModel.WeeklyInterval);
				var timeInrervalViewModel = new WeeklyIntervalViewModel(weeklyIntervalDetailsViewModel.WeeklyInterval);
				WeeklyIntervals.Add(timeInrervalViewModel);
				SelectedWeeklyInterval = timeInrervalViewModel;
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanAdd()
		{
			return WeeklyIntervals.Count < 256;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			SKDManager.SKDConfiguration.WeeklyIntervals.Remove(SelectedWeeklyInterval.WeeklyInterval);
			WeeklyIntervals.Remove(SelectedWeeklyInterval);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanDelete()
		{
			return SelectedWeeklyInterval != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var weeklyIntervalDetailsViewModel = new WeeklyIntervalDetailsViewModel(SelectedWeeklyInterval.WeeklyInterval);
			if (DialogService.ShowModalWindow(weeklyIntervalDetailsViewModel))
			{
				SelectedWeeklyInterval.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanEdit()
		{
			return SelectedWeeklyInterval != null;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			IntervalToCopy = CopyInterval(SelectedWeeklyInterval.WeeklyInterval);
		}
		bool CanCopy()
		{
			return SelectedWeeklyInterval != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var newInterval = CopyInterval(IntervalToCopy);
			SKDManager.SKDConfiguration.WeeklyIntervals.Add(newInterval);
			var timeInrervalViewModel = new WeeklyIntervalViewModel(newInterval);
			WeeklyIntervals.Add(timeInrervalViewModel);
			SelectedWeeklyInterval = timeInrervalViewModel;
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanPaste()
		{
			return IntervalToCopy != null && WeeklyIntervals.Count < 256;
		}

		SKDWeeklyInterval CopyInterval(SKDWeeklyInterval source)
		{
			var copy = new SKDWeeklyInterval();
			copy.Name = source.Name;
			foreach (var weeklyIntervalPart in source.WeeklyIntervalParts)
			{
				var copyWeeklyIntervalPart = new SKDWeeklyIntervalPart()
				{
					No = weeklyIntervalPart.No,
					IsHolliday = weeklyIntervalPart.IsHolliday,
					TimeIntervalUID = weeklyIntervalPart.TimeIntervalUID,
				};
				copy.WeeklyIntervalParts.Add(copyWeeklyIntervalPart);
			}
			return copy;
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.C, ModifierKeys.Control), CopyCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.V, ModifierKeys.Control), PasteCommand);
		}

		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Редактировать", "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("Удалить", "/Controls;component/Images/BDelete.png"),
					new RibbonMenuItemViewModel("Копировать", CopyCommand, "/Controls;component/Images/BCopy.png"),
					new RibbonMenuItemViewModel("Вставить", PasteCommand, "/Controls;component/Images/BPaste.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 1 }
			};
		}
	}
}