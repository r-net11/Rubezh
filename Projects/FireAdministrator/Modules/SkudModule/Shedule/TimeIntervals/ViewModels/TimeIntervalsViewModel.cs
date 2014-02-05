using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.ViewModels;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Ribbon;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows;
using Infrastructure;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class TimeIntervalsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public TimeIntervalsViewModel()
		{
			Menu = new TimeIntervalsMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			RegisterShortcuts();
			SetRibbonItems();

			TimeIntervals = new ObservableCollection<TimeIntervalViewModel>();
			foreach (var timeInterval in SKDManager.SKDConfiguration.TimeIntervals)
			{
				var timeInrervalViewModel = new TimeIntervalViewModel(timeInterval);
				TimeIntervals.Add(timeInrervalViewModel);
			}
			SelectedTimeInterval = TimeIntervals.FirstOrDefault();
		}

		SKDTimeInterval IntervalToCopy;

		public ObservableCollection<TimeIntervalViewModel> TimeIntervals { get; private set; }

		TimeIntervalViewModel _selectedTimeInterval;
		public TimeIntervalViewModel SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				_selectedTimeInterval = value;
				OnPropertyChanged("SelectedTimeInterval");
			}
		}

		public void Select(Guid timeIntervalUID)
		{
			if (timeIntervalUID != Guid.Empty)
			{
				var timeIntervalViewModel = TimeIntervals.FirstOrDefault(x => x.TimeInterval.UID == timeIntervalUID);
				if (timeIntervalViewModel != null)
				{
					SelectedTimeInterval = timeIntervalViewModel;
				}
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var timeIntervalDetailsViewModel = new TimeIntervalDetailsViewModel();
			if (DialogService.ShowModalWindow(timeIntervalDetailsViewModel))
			{
				SKDManager.SKDConfiguration.TimeIntervals.Add(timeIntervalDetailsViewModel.TimeInterval);
				var timeIntervalViewModel = new TimeIntervalViewModel(timeIntervalDetailsViewModel.TimeInterval);
				TimeIntervals.Add(timeIntervalViewModel);
				SelectedTimeInterval = timeIntervalViewModel;
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanAdd()
		{
			return TimeIntervals.Count < 256;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			SKDManager.SKDConfiguration.TimeIntervals.Remove(SelectedTimeInterval.TimeInterval);
			TimeIntervals.Remove(SelectedTimeInterval);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanDelete()
		{
			return SelectedTimeInterval != null && !SelectedTimeInterval.TimeInterval.IsDefault;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var timeInrervalDetailsViewModel = new TimeIntervalDetailsViewModel(SelectedTimeInterval.TimeInterval);
			if (DialogService.ShowModalWindow(timeInrervalDetailsViewModel))
			{
				SelectedTimeInterval.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanEdit()
		{
			return SelectedTimeInterval != null && !SelectedTimeInterval.TimeInterval.IsDefault;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			IntervalToCopy = CopyInterval(SelectedTimeInterval.TimeInterval);
		}
		bool CanCopy()
		{
			return SelectedTimeInterval != null && !SelectedTimeInterval.TimeInterval.IsDefault;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var newInterval = CopyInterval(IntervalToCopy);
			SKDManager.SKDConfiguration.TimeIntervals.Add(newInterval);
			var timeInrervalViewModel = new TimeIntervalViewModel(newInterval);
			TimeIntervals.Add(timeInrervalViewModel);
			SelectedTimeInterval = timeInrervalViewModel;
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanPaste()
		{
			return IntervalToCopy != null && TimeIntervals.Count < 256;
		}

		SKDTimeInterval CopyInterval(SKDTimeInterval source)
		{
			var copy = new SKDTimeInterval();
			copy.Name = source.Name;
			foreach (var timeIntervalPart in source.TimeIntervalParts)
			{
				var copyTimeIntervalPart = new SKDTimeIntervalPart()
				{
					StartTime = timeIntervalPart.StartTime,
					EndTime = timeIntervalPart.EndTime
				};
				copy.TimeIntervalParts.Add(copyTimeIntervalPart);
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