using System;
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
using Common;

namespace SKDModule.ViewModels
{
	public class TimeIntervalsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<int>
	{
		public TimeIntervalsViewModel()
		{
			Menu = new TimeIntervalsMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanEdit);
			CopyCommand = new RelayCommand(OnCopy, CanEdit);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			//var neverTimeInterval = TimeIntervals.FirstOrDefault(x => x.IsDefault);
			//if (neverTimeInterval == null)
			//{
			//    neverTimeInterval = new SKDTimeInterval();
			//    TimeIntervals.Add(neverTimeInterval);
			//    result = false;
			//}
			//neverTimeInterval.Name = "Никогда";
			//neverTimeInterval.ID = -1;
			//neverTimeInterval.TimeIntervalParts = new List<SKDTimeIntervalPart>()
			//{
			//    new SKDTimeIntervalPart() { StartTime = DateTime.MinValue, EndTime = DateTime.MinValue },
			//};
			
			TimeIntervals = new ObservableCollection<TimeIntervalViewModel>();
			var map = SKDManager.TimeIntervalsConfiguration.TimeIntervals.ToDictionary(item => item.ID);
			TimeIntervals.Add(new TimeIntervalViewModel(-1, null));
			for (int i = 0; i < 128; i++)
				TimeIntervals.Add(new TimeIntervalViewModel(i, map.ContainsKey(i) ? map[i] : null));
			SelectedTimeInterval = TimeIntervals.FirstOrDefault();
		}

		private ObservableCollection<TimeIntervalViewModel> _timeIntervals;
		public ObservableCollection<TimeIntervalViewModel> TimeIntervals
		{
			get { return _timeIntervals; }
			set
			{
				_timeIntervals = value;
				OnPropertyChanged(() => TimeIntervals);
			}
		}

		private TimeIntervalViewModel _selectedTimeInterval;
		public TimeIntervalViewModel SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				_selectedTimeInterval = value;
				OnPropertyChanged(() => SelectedTimeInterval);
			}
		}

		public void Select(int timeIntervalID)
		{
			if (timeIntervalID >= -1 && timeIntervalID < 128)
				SelectedTimeInterval = TimeIntervals.First(item => item.Index == timeIntervalID);
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
		}
		private bool CanAdd()
		{
			return false;
		}

		public RelayCommand DeleteCommand { get; private set; }
		private void OnDelete()
		{
			//SKDManager.TimeIntervalsConfiguration.TimeIntervals.Remove(SelectedTimeInterval.TimeInterval);
			//TimeIntervals.Remove(SelectedTimeInterval);
			//ServiceFactory.SaveService.SKDChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			//var timeInrervalDetailsViewModel = new TimeIntervalDetailsViewModel(SelectedTimeInterval.TimeInterval);
			//if (DialogService.ShowModalWindow(timeInrervalDetailsViewModel))
			//{
			//    SelectedTimeInterval.Update();
			//    ServiceFactory.SaveService.SKDChanged = true;
			//}
		}
		private bool CanEdit()
		{
			return SelectedTimeInterval != null && !SelectedTimeInterval.IsDefault;
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			_copy = CopyInterval(SelectedTimeInterval.TimeInterval);
		}


		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			//var newInterval = CopyInterval(_copy);
			//SelectedTimeInterval.TimeIntervalParts = newInterval.TimeIntervalParts;
			//SKDManager.TimeIntervalsConfiguration.TimeIntervals.Add(newInterval);
			//var timeInrervalViewModel = new TimeIntervalViewModel(newInterval);
			//TimeIntervals.Add(timeInrervalViewModel);
			//SelectedTimeInterval = timeInrervalViewModel;
			//ServiceFactory.SaveService.SKDChanged = true;
		}
		private bool CanPaste()
		{
			return _copy != null && CanEdit();
		}

		private SKDTimeInterval _copy;
		private SKDTimeInterval CopyInterval(SKDTimeInterval source)
		{
			var copy = new SKDTimeInterval();
			//copy.Name = source.Name;
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
					new RibbonMenuItemViewModel("Редактировать", "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("Сбросить", "/Controls;component/Images/BDelete.png"),
					new RibbonMenuItemViewModel("Копировать", CopyCommand, "/Controls;component/Images/BCopy.png"),
					new RibbonMenuItemViewModel("Вставить", PasteCommand, "/Controls;component/Images/BPaste.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 1 }
			};
		}
	}
}