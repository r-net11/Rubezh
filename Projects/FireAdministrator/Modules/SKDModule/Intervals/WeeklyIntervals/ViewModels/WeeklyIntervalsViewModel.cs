using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common;
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
			CopyCommand = new RelayCommand(OnCopy, CanEdit);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			ActivateCommand = new RelayCommand(OnActivate, CanActivate);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			BuildIntervals();
			var map = SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.ToDictionary(item => item.ID);
			WeeklyIntervals = new ObservableCollection<WeeklyIntervalViewModel>();
			for (int i = 1; i <= 128; i++)
				WeeklyIntervals.Add(new WeeklyIntervalViewModel(i, map.ContainsKey(i) ? map[i] : null, this));
			SelectedWeeklyInterval = WeeklyIntervals.First();
		}

		private ObservableCollection<WeeklyIntervalViewModel> _weeklyIntervals;
		public ObservableCollection<WeeklyIntervalViewModel> WeeklyIntervals
		{
			get { return _weeklyIntervals; }
			set
			{
				_weeklyIntervals = value;
				OnPropertyChanged(() => WeeklyIntervals);
			}
		}

		private WeeklyIntervalViewModel _selectedWeeklyInterval;
		public WeeklyIntervalViewModel SelectedWeeklyInterval
		{
			get { return _selectedWeeklyInterval; }
			set
			{
				if (value == null)
					SelectedWeeklyInterval = WeeklyIntervals.First();
				else
				{
					_selectedWeeklyInterval = value;
					OnPropertyChanged(() => SelectedWeeklyInterval);
					SelectedWeeklyInterval.TimeIntervals.ForEach(item => item.Update());
					UpdateRibbonItems();
				}
			}
		}

		public ObservableCollection<SKDTimeInterval> AvailableTimeIntervals { get; private set; }

		public void Select(int intervalID)
		{
			if (intervalID >= 0 && intervalID <= 128)
				SelectedWeeklyInterval = WeeklyIntervals.First(item => item.Index == intervalID);
			else if (SelectedWeeklyInterval == null)
				SelectedWeeklyInterval = WeeklyIntervals.First();
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
			var weeklyIntervalDetailsViewModel = new WeeklyIntervalDetailsViewModel(SelectedWeeklyInterval.WeeklyInterval);
			if (DialogService.ShowModalWindow(weeklyIntervalDetailsViewModel))
			{
				SelectedWeeklyInterval.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		private bool CanEdit()
		{
			return SelectedWeeklyInterval != null && SelectedWeeklyInterval.IsEnabled;
		}

		public RelayCommand ActivateCommand { get; private set; }
		private void OnActivate()
		{
			SelectedWeeklyInterval.IsActive = !SelectedWeeklyInterval.IsActive;
			OnPropertyChanged(() => SelectedWeeklyInterval);
			UpdateRibbonItems();
		}
		private bool CanActivate()
		{
			return SelectedWeeklyInterval != null && !SelectedWeeklyInterval.IsDefault;
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			_copy = CopyInterval(SelectedWeeklyInterval.WeeklyInterval);
		}

		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			var newInterval = CopyInterval(_copy);
			SelectedWeeklyInterval.Paste(newInterval);
		}
		private bool CanPaste()
		{
			return _copy != null && SelectedWeeklyInterval != null && !SelectedWeeklyInterval.IsDefault;
		}

		private SKDWeeklyInterval _copy;
		private SKDWeeklyInterval CopyInterval(SKDWeeklyInterval source)
		{
			var copy = new SKDWeeklyInterval()
			{
				Name = source.Name,
				Description = source.Description,
			};
			for (int i = 0; i < source.WeeklyIntervalParts.Count; i++)
				copy.WeeklyIntervalParts[i].TimeIntervalID = source.WeeklyIntervalParts[i].TimeIntervalID;
			return copy;
		}

		public override void OnShow()
		{
			BuildIntervals();
			base.OnShow();
		}
		private void BuildIntervals()
		{
			AvailableTimeIntervals = new ObservableCollection<SKDTimeInterval>(SKDManager.TimeIntervalsConfiguration.TimeIntervals.OrderBy(item => item.ID));
			AvailableTimeIntervals.Insert(0, new SKDTimeInterval()
			{
				ID = 0,
				Name = "Никогда",
			});
			OnPropertyChanged(() => AvailableTimeIntervals);
			if (SelectedWeeklyInterval != null)
				SelectedWeeklyInterval.TimeIntervals.ForEach(item => item.Update());
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.C, ModifierKeys.Control), CopyCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.V, ModifierKeys.Control), PasteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.A, ModifierKeys.Control), ActivateCommand);
		}

		protected override void UpdateRibbonItems()
		{
			base.UpdateRibbonItems();
			RibbonItems[0][1].Text = SelectedWeeklyInterval.ActivateActionTitle;
			RibbonItems[0][1].ImageSource = SelectedWeeklyInterval.GetActiveImage(!SelectedWeeklyInterval.IsActive, true);
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