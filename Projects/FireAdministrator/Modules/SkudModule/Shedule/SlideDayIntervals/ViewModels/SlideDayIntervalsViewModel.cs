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

namespace SkudModule.ViewModels
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

			SlideDayIntervals = new ObservableCollection<SlideDayIntervalViewModel>();
			foreach (var slideDayInterval in SKDManager.SKDConfiguration.SlideDayIntervals)
			{
				var timeInrervalViewModel = new SlideDayIntervalViewModel(slideDayInterval);
				SlideDayIntervals.Add(timeInrervalViewModel);
			}
			SelectedSlideDayInterval = SlideDayIntervals.FirstOrDefault();
		}

		SKDSlideDayInterval IntervalToCopy;

		public ObservableCollection<SlideDayIntervalViewModel> SlideDayIntervals { get; private set; }

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
			return SelectedSlideDayInterval != null;
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
			return SelectedSlideDayInterval != null;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			IntervalToCopy = CopyInterval(SelectedSlideDayInterval.SlideDayInterval);
		}
		bool CanCopy()
		{
			return SelectedSlideDayInterval != null;
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