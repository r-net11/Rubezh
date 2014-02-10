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
	public class SlideWeekIntervalsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public SlideWeekIntervalsViewModel()
		{
			Menu = new SlideWeekIntervalsMenuViewModel(this);
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
			SlideWeekIntervals = new ObservableCollection<SlideWeekIntervalViewModel>();
			foreach (var slideWeekInterval in SKDManager.SKDConfiguration.SlideWeeklyIntervals)
			{
				var timeInrervalViewModel = new SlideWeekIntervalViewModel(slideWeekInterval);
				SlideWeekIntervals.Add(timeInrervalViewModel);
			}
			SelectedSlideWeekInterval = SlideWeekIntervals.FirstOrDefault();
		}

		SKDSlideWeeklyInterval IntervalToCopy;

		ObservableCollection<SlideWeekIntervalViewModel> _slideWeekIntervals;
		public ObservableCollection<SlideWeekIntervalViewModel> SlideWeekIntervals
		{
			get { return _slideWeekIntervals; }
			set
			{
				_slideWeekIntervals = value;
				OnPropertyChanged("SlideWeekIntervals");
			}
		}

		SlideWeekIntervalViewModel _selectedSlideWeekInterval;
		public SlideWeekIntervalViewModel SelectedSlideWeekInterval
		{
			get { return _selectedSlideWeekInterval; }
			set
			{
				_selectedSlideWeekInterval = value;
				OnPropertyChanged("SelectedSlideWeekInterval");
				if (value != null)
				{
					value.Initialize();
				}
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
				SKDManager.SKDConfiguration.SlideWeeklyIntervals.Add(slideWeekIntervalDetailsViewModel.SlideWeekInterval);
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

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			SKDManager.SKDConfiguration.SlideWeeklyIntervals.Remove(SelectedSlideWeekInterval.SlideWeekInterval);
			SlideWeekIntervals.Remove(SelectedSlideWeekInterval);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanDelete()
		{
			return SelectedSlideWeekInterval != null && !SelectedSlideWeekInterval.SlideWeekInterval.IsDefault;
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
			return SelectedSlideWeekInterval != null && !SelectedSlideWeekInterval.SlideWeekInterval.IsDefault;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			IntervalToCopy = CopyInterval(SelectedSlideWeekInterval.SlideWeekInterval);
		}
		bool CanCopy()
		{
			return SelectedSlideWeekInterval != null && !SelectedSlideWeekInterval.SlideWeekInterval.IsDefault;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var newInterval = CopyInterval(IntervalToCopy);
			SKDManager.SKDConfiguration.SlideWeeklyIntervals.Add(newInterval);
			var timeInrervalViewModel = new SlideWeekIntervalViewModel(newInterval);
			SlideWeekIntervals.Add(timeInrervalViewModel);
			SelectedSlideWeekInterval = timeInrervalViewModel;
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanPaste()
		{
			return IntervalToCopy != null && SlideWeekIntervals.Count < 256;
		}

		SKDSlideWeeklyInterval CopyInterval(SKDSlideWeeklyInterval source)
		{
			var copy = new SKDSlideWeeklyInterval();
			copy.Name = source.Name;
			foreach (var timeIntervalUID in source.WeeklyIntervalUIDs)
			{
				copy.WeeklyIntervalUIDs.Add(timeIntervalUID);
			}
			return copy;
		}

		public override void OnShow()
		{
			SelectedSlideWeekInterval = SelectedSlideWeekInterval;
			base.OnShow();
		}

		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.C, ModifierKeys.Control), CopyCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.V, ModifierKeys.Control), PasteCommand);
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
					new RibbonMenuItemViewModel("Копировать", CopyCommand, "/Controls;component/Images/BCopy.png"),
					new RibbonMenuItemViewModel("Вставить", PasteCommand, "/Controls;component/Images/BPaste.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 1 }
			};
		}
	}
}