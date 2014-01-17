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
	public class SlideWeekIntervalsViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public SlideWeekIntervalsViewModel()
		{
			Menu = new SlideWeekIntervalsMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			RegisterShortcuts();
			SetRibbonItems();

			SlideWeekIntervals = new ObservableCollection<SlideWeekIntervalViewModel>();
			foreach (var slideWeekInterval in SKDManager.SKDConfiguration.SlideWeekIntervals)
			{
				var timeInrervalViewModel = new SlideWeekIntervalViewModel(slideWeekInterval);
				SlideWeekIntervals.Add(timeInrervalViewModel);
			}
			SelectedSlideWeekInterval = SlideWeekIntervals.FirstOrDefault();
		}

		SKDSlideWeekInterval IntervalToCopy;

		public ObservableCollection<SlideWeekIntervalViewModel> SlideWeekIntervals { get; private set; }

		SlideWeekIntervalViewModel _selectedSlideWeekInterval;
		public SlideWeekIntervalViewModel SelectedSlideWeekInterval
		{
			get { return _selectedSlideWeekInterval; }
			set
			{
				_selectedSlideWeekInterval = value;
				OnPropertyChanged("SelectedSlideWeekInterval");
			}
		}

		public void Select(Guid intervalUID)
		{
			if (intervalUID != Guid.Empty)
			{
				var intervalViewModel = SlideWeekIntervals.FirstOrDefault(x => x.SlideWeekInterval.UID == intervalUID);
				if (intervalViewModel != null)
				{
					SelectedSlideWeekInterval = intervalViewModel;
				}
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var slideWeekIntervalDetailsViewModel = new SlideWeekIntervalDetailsViewModel();
			if (DialogService.ShowModalWindow(slideWeekIntervalDetailsViewModel))
			{
				SKDManager.SKDConfiguration.SlideWeekIntervals.Add(slideWeekIntervalDetailsViewModel.SlideWeekInterval);
				var timeInrervalViewModel = new SlideWeekIntervalViewModel(slideWeekIntervalDetailsViewModel.SlideWeekInterval);
				SlideWeekIntervals.Add(timeInrervalViewModel);
				SelectedSlideWeekInterval = timeInrervalViewModel;
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanAdd()
		{
			return SlideWeekIntervals.Count < 256;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			SKDManager.SKDConfiguration.SlideWeekIntervals.Remove(SelectedSlideWeekInterval.SlideWeekInterval);
			SlideWeekIntervals.Remove(SelectedSlideWeekInterval);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanRemove()
		{
			return SelectedSlideWeekInterval != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var slideWeekIntervalDetailsViewModel = new SlideWeekIntervalDetailsViewModel(SelectedSlideWeekInterval.SlideWeekInterval);
			if (DialogService.ShowModalWindow(slideWeekIntervalDetailsViewModel))
			{
				SelectedSlideWeekInterval.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanEdit()
		{
			return SelectedSlideWeekInterval != null;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			IntervalToCopy = CopyInterval(SelectedSlideWeekInterval.SlideWeekInterval);
		}
		bool CanCopy()
		{
			return SelectedSlideWeekInterval != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var newInterval = CopyInterval(IntervalToCopy);
			SKDManager.SKDConfiguration.SlideWeekIntervals.Add(newInterval);
			var timeInrervalViewModel = new SlideWeekIntervalViewModel(newInterval);
			SlideWeekIntervals.Add(timeInrervalViewModel);
			SelectedSlideWeekInterval = timeInrervalViewModel;
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanPaste()
		{
			return IntervalToCopy != null && SlideWeekIntervals.Count < 256;
		}

		SKDSlideWeekInterval CopyInterval(SKDSlideWeekInterval source)
		{
			var copy = new SKDSlideWeekInterval();
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
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), RemoveCommand);
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