using System;
using Common;
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
	public class SlideWeekIntervalsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<int>
	{
		public SlideWeekIntervalsViewModel()
		{
			Menu = new SlideWeekIntervalsMenuViewModel(this);
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
			var map = SKDManager.TimeIntervalsConfiguration.SlideWeeklyIntervals.ToDictionary(item => item.ID);
			SlideWeekIntervals = new ObservableCollection<SlideWeekIntervalViewModel>();
			for (int i = 1; i <= 128; i++)
				SlideWeekIntervals.Add(new SlideWeekIntervalViewModel(i, map.ContainsKey(i) ? map[i] : null, this));
			SelectedSlideWeekInterval = SlideWeekIntervals.First();
		}

		private ObservableCollection<SlideWeekIntervalViewModel> _slideWeekIntervals;
		public ObservableCollection<SlideWeekIntervalViewModel> SlideWeekIntervals
		{
			get { return _slideWeekIntervals; }
			set
			{
				_slideWeekIntervals = value;
				OnPropertyChanged(() => SlideWeekIntervals);
			}
		}

		private SlideWeekIntervalViewModel _selectedSlideWeekInterval;
		public SlideWeekIntervalViewModel SelectedSlideWeekInterval
		{
			get { return _selectedSlideWeekInterval; }
			set
			{
				if (value == null)
					_selectedSlideWeekInterval = SlideWeekIntervals.First();
				else
				{
					_selectedSlideWeekInterval = value;
					OnPropertyChanged(() => SelectedSlideWeekInterval);
					SelectedSlideWeekInterval.WeekIntervals.ForEach(item => item.Update());
					UpdateRibbonItems();
				}
			}
		}

		public ObservableCollection<SKDWeeklyInterval> AvailableWeekIntervals { get; private set; }

		public void Select(int intervalID)
		{
			if (intervalID >= 0 && intervalID <= 128)
				SelectedSlideWeekInterval = SlideWeekIntervals.First(item => item.Index == intervalID);
			else if (SelectedSlideWeekInterval == null)
				SelectedSlideWeekInterval = SlideWeekIntervals.First();
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
			var slideWeekIntervalDetailsViewModel = new SlideWeekIntervalDetailsViewModel(SelectedSlideWeekInterval.SlideWeekInterval);
			if (DialogService.ShowModalWindow(slideWeekIntervalDetailsViewModel))
			{
				SelectedSlideWeekInterval.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		private bool CanEdit()
		{
			return SelectedSlideWeekInterval != null && SelectedSlideWeekInterval.IsEnabled;
		}

		public RelayCommand ActivateCommand { get; private set; }
		private void OnActivate()
		{
			SelectedSlideWeekInterval.IsActive = !SelectedSlideWeekInterval.IsActive;
			OnPropertyChanged(() => SelectedSlideWeekInterval);
			UpdateRibbonItems();
		}
		private bool CanActivate()
		{
			return SelectedSlideWeekInterval != null && !SelectedSlideWeekInterval.IsDefault;
		}


		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			_copy = CopyInterval(SelectedSlideWeekInterval.SlideWeekInterval);
		}

		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			var newInterval = CopyInterval(_copy);
			SelectedSlideWeekInterval.Paste(newInterval);
		}
		private bool CanPaste()
		{
			return _copy != null && SelectedSlideWeekInterval != null && !SelectedSlideWeekInterval.IsDefault;
		}

		private SKDSlideWeeklyInterval _copy;
		private SKDSlideWeeklyInterval CopyInterval(SKDSlideWeeklyInterval source)
		{
			return new SKDSlideWeeklyInterval()
			{
				Name = source.Name,
				Description = source.Description,
				StartDate = source.StartDate,
				WeeklyIntervalIDs = new List<int>(source.WeeklyIntervalIDs),
			};
		}

		public override void OnShow()
		{
			BuildIntervals();
			base.OnShow();
		}
		private void BuildIntervals()
		{
			AvailableWeekIntervals = new ObservableCollection<SKDWeeklyInterval>(SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.OrderBy(item => item.ID));
			AvailableWeekIntervals.Insert(0, new SKDWeeklyInterval()
			{
				ID = 0,
				Name = "Никогда",
			});
			OnPropertyChanged(() => AvailableWeekIntervals);
			if (SelectedSlideWeekInterval != null)
				SelectedSlideWeekInterval.WeekIntervals.ForEach(item => item.Update());
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
			RibbonItems[0][1].Text = SelectedSlideWeekInterval.ActivateActionTitle;
			RibbonItems[0][1].ImageSource = SelectedSlideWeekInterval.GetActiveImage(!SelectedSlideWeekInterval.IsActive, true);
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