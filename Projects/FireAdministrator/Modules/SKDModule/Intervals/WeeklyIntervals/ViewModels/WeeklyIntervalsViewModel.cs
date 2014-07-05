using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace SKDModule.ViewModels
{
	public class WeeklyIntervalsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<int>
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
		}

		public void Initialize()
		{
			WeeklyIntervals = new ObservableCollection<WeeklyIntervalViewModel>();
			foreach (var weeklyInterval in SKDManager.TimeIntervalsConfiguration.WeeklyIntervals)
			{
				var timeInrervalViewModel = new WeeklyIntervalViewModel(weeklyInterval);
				WeeklyIntervals.Add(timeInrervalViewModel);
			}
			SelectedWeeklyInterval = WeeklyIntervals.FirstOrDefault();
		}

		private ObservableCollection<WeeklyIntervalViewModel> _weeklyIntervals;
		public ObservableCollection<WeeklyIntervalViewModel> WeeklyIntervals
		{
			get { return _weeklyIntervals; }
			set
			{
				_weeklyIntervals = value;
				OnPropertyChanged("WeeklyIntervals");
			}
		}

		private WeeklyIntervalViewModel _selectedWeeklyInterval;
		public WeeklyIntervalViewModel SelectedWeeklyInterval
		{
			get { return _selectedWeeklyInterval; }
			set
			{
				_selectedWeeklyInterval = value;
				OnPropertyChanged("SelectedWeeklyInterval");
				if (value != null)
				{
					value.Initialize();
				}
			}
		}

		public void Select(int intervalID)
		{
			if (intervalID > 0)
			{
				var intervalViewModel = WeeklyIntervals.FirstOrDefault(x => x.WeeklyInterval.ID == intervalID);
				if (intervalViewModel != null)
				{
					SelectedWeeklyInterval = intervalViewModel;
				}
			}
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var weeklyIntervalDetailsViewModel = new WeeklyIntervalDetailsViewModel();
			if (DialogService.ShowModalWindow(weeklyIntervalDetailsViewModel))
			{
				SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.Add(weeklyIntervalDetailsViewModel.WeeklyInterval);
				var timeInrervalViewModel = new WeeklyIntervalViewModel(weeklyIntervalDetailsViewModel.WeeklyInterval);
				WeeklyIntervals.Add(timeInrervalViewModel);
				SelectedWeeklyInterval = timeInrervalViewModel;
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		private bool CanAdd()
		{
			return WeeklyIntervals.Count < 256;
		}

		public RelayCommand DeleteCommand { get; private set; }
		private void OnDelete()
		{
			SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.Remove(SelectedWeeklyInterval.WeeklyInterval);
			WeeklyIntervals.Remove(SelectedWeeklyInterval);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		private bool CanDelete()
		{
			//return SelectedWeeklyInterval != null && !SelectedWeeklyInterval.WeeklyInterval.IsDefault;
			return true;
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			var weeklyIntervalDetailsViewModel = new WeeklyIntervalDetailsViewModel(SelectedWeeklyInterval.WeeklyInterval);
			if (DialogService.ShowModalWindow(weeklyIntervalDetailsViewModel))
			{
				SelectedWeeklyInterval.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		private bool CanEdit()
		{
			//return SelectedWeeklyInterval != null && !SelectedWeeklyInterval.WeeklyInterval.IsDefault;
			return true;
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			_copy = CopyInterval(SelectedWeeklyInterval.WeeklyInterval);
		}
		private bool CanCopy()
		{
			//return SelectedWeeklyInterval != null && !SelectedWeeklyInterval.WeeklyInterval.IsDefault;
			return true;
		}

		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			var newInterval = CopyInterval(_copy);
			SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.Add(newInterval);
			var timeInrervalViewModel = new WeeklyIntervalViewModel(newInterval);
			WeeklyIntervals.Add(timeInrervalViewModel);
			SelectedWeeklyInterval = timeInrervalViewModel;
			ServiceFactory.SaveService.SKDChanged = true;
		}
		private bool CanPaste()
		{
			return _copy != null && WeeklyIntervals.Count < 256;
		}

		private SKDWeeklyInterval _copy;
		private SKDWeeklyInterval CopyInterval(SKDWeeklyInterval source)
		{
			var copy = new SKDWeeklyInterval();
			copy.Name = source.Name;
			foreach (var weeklyIntervalPart in source.WeeklyIntervalParts)
			{
				var copyWeeklyIntervalPart = new SKDWeeklyIntervalPart()
				{
					No = weeklyIntervalPart.No,
					IsHolliday = weeklyIntervalPart.IsHolliday,
					TimeIntervalID = weeklyIntervalPart.TimeIntervalID,
				};
				copy.WeeklyIntervalParts.Add(copyWeeklyIntervalPart);
			}
			return copy;
		}

		public override void OnShow()
		{
			SelectedWeeklyInterval = SelectedWeeklyInterval;
			base.OnShow();
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