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
	public class SlideDayIntervalsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<int>
	{
		public SlideDayIntervalsViewModel()
		{
			Menu = new SlideDayIntervalsMenuViewModel(this);
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
			var map = SKDManager.TimeIntervalsConfiguration.SlideDayIntervals.ToDictionary(item => item.ID);
			SlideDayIntervals = new ObservableCollection<SlideDayIntervalViewModel>();
			for (int i = 1; i <= 128; i++)
				SlideDayIntervals.Add(new SlideDayIntervalViewModel(i, map.ContainsKey(i) ? map[i] : null, this));
			SelectedSlideDayInterval = SlideDayIntervals.First();
		}

		private ObservableCollection<SlideDayIntervalViewModel> _slideDayIntervals;
		public ObservableCollection<SlideDayIntervalViewModel> SlideDayIntervals
		{
			get { return _slideDayIntervals; }
			set
			{
				_slideDayIntervals = value;
				OnPropertyChanged(() => SlideDayIntervals);
			}
		}

		private SlideDayIntervalViewModel _selectedSlideDayInterval;
		public SlideDayIntervalViewModel SelectedSlideDayInterval
		{
			get { return _selectedSlideDayInterval; }
			set
			{
				if (value == null)
					_selectedSlideDayInterval = SlideDayIntervals.First();
				else
				{
					_selectedSlideDayInterval = value;
					OnPropertyChanged(() => SelectedSlideDayInterval);
					SelectedSlideDayInterval.TimeIntervals.ForEach(item => item.Update());
					UpdateRibbonItems();
				}
			}
		}

		public ObservableCollection<SKDTimeInterval> AvailableTimeIntervals { get; private set; }

		public void Select(int intervalID)
		{
			if (intervalID >= 0 && intervalID <= 128)
				SelectedSlideDayInterval = SlideDayIntervals.First(item => item.Index == intervalID);
			else if (SelectedSlideDayInterval == null)
				SelectedSlideDayInterval = SlideDayIntervals.First();
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
			var slideDayIntervalDetailsViewModel = new SlideDayIntervalDetailsViewModel(SelectedSlideDayInterval.SlideDayInterval);
			if (DialogService.ShowModalWindow(slideDayIntervalDetailsViewModel))
			{
				SelectedSlideDayInterval.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		private bool CanEdit()
		{
			return SelectedSlideDayInterval != null && SelectedSlideDayInterval.IsEnabled;
		}

		public RelayCommand ActivateCommand { get; private set; }
		private void OnActivate()
		{
			SelectedSlideDayInterval.IsActive = !SelectedSlideDayInterval.IsActive;
			OnPropertyChanged(() => SelectedSlideDayInterval);
			UpdateRibbonItems();
		}
		private bool CanActivate()
		{
			return SelectedSlideDayInterval != null && !SelectedSlideDayInterval.IsDefault;
		}


		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			_copy = CopyInterval(SelectedSlideDayInterval.SlideDayInterval);
		}

		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			var newInterval = CopyInterval(_copy);
			SelectedSlideDayInterval.Paste(newInterval);
		}
		private bool CanPaste()
		{
			return _copy != null && SelectedSlideDayInterval != null && !SelectedSlideDayInterval.IsDefault;
		}

		private SKDSlideDayInterval _copy;
		private SKDSlideDayInterval CopyInterval(SKDSlideDayInterval source)
		{
			return new SKDSlideDayInterval()
			{
				Name = source.Name,
				Description = source.Description,
				StartDate = source.StartDate,
				TimeIntervalIDs = new List<int>(source.TimeIntervalIDs),
			};
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
			if (SelectedSlideDayInterval != null)
				SelectedSlideDayInterval.TimeIntervals.ForEach(item => item.Update());
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
			RibbonItems[0][1].Text = SelectedSlideDayInterval.ActivateActionTitle;
			RibbonItems[0][1].ImageSource = SelectedSlideDayInterval.GetActiveImage(!SelectedSlideDayInterval.IsActive, true);
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