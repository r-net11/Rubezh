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
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			CopyCommand = new RelayCommand(OnCopy, CanEdit);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			ActivateCommand = new RelayCommand(OnActivate, CanActivate);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			TimeIntervals = new ObservableCollection<TimeIntervalViewModel>();
			var map = SKDManager.TimeIntervalsConfiguration.TimeIntervals.ToDictionary(item => item.ID);
			//TimeIntervals.Add(new TimeIntervalViewModel(0, null));
			for (int i = 1; i <= 128; i++)
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
				if (value == null)
					SelectedTimeInterval = TimeIntervals.FirstOrDefault(item => item.IsDefault) ?? TimeIntervals.First();
				else
				{
					_selectedTimeInterval = value;
					OnPropertyChanged(() => SelectedTimeInterval);
					UpdateRibbonItems();
				}
			}
		}

		public void Select(int timeIntervalID)
		{
			SelectedTimeInterval = TimeIntervals.First(item => timeIntervalID >= 1 && timeIntervalID <= 128 ? item.Index == timeIntervalID : true);
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
		}
		private bool CanDelete()
		{
			return false;
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			var timeInrervalDetailsViewModel = new TimeIntervalDetailsViewModel(SelectedTimeInterval.TimeInterval);
			if (DialogService.ShowModalWindow(timeInrervalDetailsViewModel))
			{
				SelectedTimeInterval.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		private bool CanEdit()
		{
			return SelectedTimeInterval != null && SelectedTimeInterval.IsEnabled;
		}

		public RelayCommand ActivateCommand { get; private set; }
		private void OnActivate()
		{
			SelectedTimeInterval.IsActive = !SelectedTimeInterval.IsActive;
			OnPropertyChanged(() => SelectedTimeInterval);
			UpdateRibbonItems();
		}
		private bool CanActivate()
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
			var newInterval = CopyInterval(_copy);
			SelectedTimeInterval.Paste(newInterval);
		}
		private bool CanPaste()
		{
			return _copy != null && SelectedTimeInterval != null && !SelectedTimeInterval.IsDefault;
		}

		private SKDTimeInterval _copy;
		private SKDTimeInterval CopyInterval(SKDTimeInterval source)
		{
			var copy = new SKDTimeInterval();
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
			RegisterShortcut(new KeyGesture(KeyboardKey.A, ModifierKeys.Control), ActivateCommand);
		}

		protected override void UpdateRibbonItems()
		{
			base.UpdateRibbonItems();
			RibbonItems[0][1].Text = SelectedTimeInterval.ActivateActionTitle;
			RibbonItems[0][1].ImageSource = SelectedTimeInterval.GetActiveImage(!SelectedTimeInterval.IsActive, true);
		}
		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Редактировать", "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("", ActivateCommand),
					new RibbonMenuItemViewModel("Копировать", CopyCommand, "/Controls;component/Images/BCopy.png"),
					new RibbonMenuItemViewModel("Вставить", PasteCommand, "/Controls;component/Images/BPaste.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 1 }
			};
		}
	}
}