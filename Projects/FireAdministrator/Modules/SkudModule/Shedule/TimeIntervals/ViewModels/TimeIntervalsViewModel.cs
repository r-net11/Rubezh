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

			TimeIntervals = new ObservableCollection<NamedTimeIntervalViewModel>();
			foreach (var namedTimeInterval in SKDManager.SKDConfiguration.NamedTimeIntervals)
			{
				var timeInrervalViewModel = new NamedTimeIntervalViewModel(namedTimeInterval);
				TimeIntervals.Add(timeInrervalViewModel);
			}
			SelectedTimeInterval = TimeIntervals.FirstOrDefault();
		}

		NamedSKDTimeInterval IntervalToCopy;

		public ObservableCollection<NamedTimeIntervalViewModel> TimeIntervals { get; private set; }

		NamedTimeIntervalViewModel _selectedTimeInterval;
		public NamedTimeIntervalViewModel SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				_selectedTimeInterval = value;
				OnPropertyChanged("SelectedTimeInterval");
			}
		}

		public void Select(Guid intervalUID)
		{
			if (intervalUID != Guid.Empty)
			{
				var intervalViewModel = TimeIntervals.FirstOrDefault(x => x.NamedTimeInterval.UID == intervalUID);
				if (intervalViewModel != null)
				{
					SelectedTimeInterval = intervalViewModel;
				}
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var namedTimeInrervalDetailsViewModel = new NamedTimeIntervalDetailsViewModel();
			if (DialogService.ShowModalWindow(namedTimeInrervalDetailsViewModel))
			{
				SKDManager.SKDConfiguration.NamedTimeIntervals.Add(namedTimeInrervalDetailsViewModel.NamedTimeInterval);
				var timeInrervalViewModel = new NamedTimeIntervalViewModel(namedTimeInrervalDetailsViewModel.NamedTimeInterval);
				TimeIntervals.Add(timeInrervalViewModel);
				SelectedTimeInterval = timeInrervalViewModel;
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
			SKDManager.SKDConfiguration.NamedTimeIntervals.Remove(SelectedTimeInterval.NamedTimeInterval);
			TimeIntervals.Remove(SelectedTimeInterval);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanDelete()
		{
			return SelectedTimeInterval != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var namedTimeInrervalDetailsViewModel = new NamedTimeIntervalDetailsViewModel(SelectedTimeInterval.NamedTimeInterval);
			if (DialogService.ShowModalWindow(namedTimeInrervalDetailsViewModel))
			{
				SelectedTimeInterval.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanEdit()
		{
			return SelectedTimeInterval != null;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			IntervalToCopy = CopyInterval(SelectedTimeInterval.NamedTimeInterval);
		}
		bool CanCopy()
		{
			return SelectedTimeInterval != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var newInterval = CopyInterval(IntervalToCopy);
			SKDManager.SKDConfiguration.NamedTimeIntervals.Add(newInterval);
			var timeInrervalViewModel = new NamedTimeIntervalViewModel(newInterval);
			TimeIntervals.Add(timeInrervalViewModel);
			SelectedTimeInterval = timeInrervalViewModel;
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanPaste()
		{
			return IntervalToCopy != null && TimeIntervals.Count < 256;
		}

		NamedSKDTimeInterval CopyInterval(NamedSKDTimeInterval source)
		{
			var copy = new NamedSKDTimeInterval();
			copy.Name = source.Name;
			foreach (var timeInterval in source.TimeIntervals)
			{
				var copyTimeInterval = new SKDTimeInterval()
				{
					StartTime = timeInterval.StartTime,
					EndTime = timeInterval.EndTime
				};
				copy.TimeIntervals.Add(copyTimeInterval);
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