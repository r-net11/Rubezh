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
	public class SlideDayIntervalsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public SlideDayIntervalsViewModel()
		{
			Menu = new SlideDayIntervalsMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			SlideDayIntervals = new ObservableCollection<SlideDayIntervalViewModel>();
			foreach (var slideDayInterval in SKDManager.SKDConfiguration.SlideDayIntervals)
			{
				var timeInrervalViewModel = new SlideDayIntervalViewModel(slideDayInterval);
				SlideDayIntervals.Add(timeInrervalViewModel);
			}
			SelectedSlideDayInterval = SlideDayIntervals.FirstOrDefault();
		}

		SKDSlideDayInterval IntervalToCopy;

		ObservableCollection<SlideDayIntervalViewModel> _slideDayIntervals;
		public ObservableCollection<SlideDayIntervalViewModel> SlideDayIntervals
		{
			get { return _slideDayIntervals; }
			set
			{
				_slideDayIntervals = value;
				OnPropertyChanged("SlideDayIntervals");
			}
		}

		SlideDayIntervalViewModel _selectedSlideDayInterval;
		public SlideDayIntervalViewModel SelectedSlideDayInterval
		{
			get { return _selectedSlideDayInterval; }
			set
			{
				_selectedSlideDayInterval = value;
				OnPropertyChanged("SelectedSlideDayInterval");
			}
		}

		public void Select(Guid intervalUID)
		{
			if (intervalUID != Guid.Empty)
			{
				var intervalViewModel = SlideDayIntervals.FirstOrDefault(x => x.SlideDayInterval.UID == intervalUID);
				if (intervalViewModel != null)
				{
					SelectedSlideDayInterval = intervalViewModel;
				}
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var slideDayIntervalDetailsViewModel = new SlideDayIntervalDetailsViewModel();
			if (DialogService.ShowModalWindow(slideDayIntervalDetailsViewModel))
			{
				SKDManager.SKDConfiguration.SlideDayIntervals.Add(slideDayIntervalDetailsViewModel.SlideDayInterval);
				var timeInrervalViewModel = new SlideDayIntervalViewModel(slideDayIntervalDetailsViewModel.SlideDayInterval);
				SlideDayIntervals.Add(timeInrervalViewModel);
				SelectedSlideDayInterval = timeInrervalViewModel;
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanAdd()
		{
			return SlideDayIntervals.Count < 256;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			SKDManager.SKDConfiguration.SlideDayIntervals.Remove(SelectedSlideDayInterval.SlideDayInterval);
			SlideDayIntervals.Remove(SelectedSlideDayInterval);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanDelete()
		{
			return SelectedSlideDayInterval != null && !SelectedSlideDayInterval.SlideDayInterval.IsDefault;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var slideDayIntervalDetailsViewModel = new SlideDayIntervalDetailsViewModel(SelectedSlideDayInterval.SlideDayInterval);
			if (DialogService.ShowModalWindow(slideDayIntervalDetailsViewModel))
			{
				SelectedSlideDayInterval.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanEdit()
		{
			return SelectedSlideDayInterval != null && !SelectedSlideDayInterval.SlideDayInterval.IsDefault;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			IntervalToCopy = CopyInterval(SelectedSlideDayInterval.SlideDayInterval);
		}
		bool CanCopy()
		{
			return SelectedSlideDayInterval != null && !SelectedSlideDayInterval.SlideDayInterval.IsDefault;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var newInterval = CopyInterval(IntervalToCopy);
			SKDManager.SKDConfiguration.SlideDayIntervals.Add(newInterval);
			var timeInrervalViewModel = new SlideDayIntervalViewModel(newInterval);
			SlideDayIntervals.Add(timeInrervalViewModel);
			SelectedSlideDayInterval = timeInrervalViewModel;
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanPaste()
		{
			return IntervalToCopy != null && SlideDayIntervals.Count < 256;
		}

		SKDSlideDayInterval CopyInterval(SKDSlideDayInterval source)
		{
			var copy = new SKDSlideDayInterval();
			copy.Name = source.Name;
			foreach (var timeIntervalUID in source.TimeIntervalUIDs)
			{
				copy.TimeIntervalUIDs.Add(timeIntervalUID);
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