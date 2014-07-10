using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using Common;

namespace SKDModule.Intervals.Base.ViewModels
{
	public class BaseIntervalsViewModel<TInterval, TPart, TModel> : MenuViewPartViewModel, ISelectable<int>, IEditingBaseViewModel
		where TInterval : BaseIntervalViewModel<TPart, TModel>
		where TPart : BaseIntervalPartViewModel
	{
		public BaseIntervalsViewModel()
		{
			Menu = new BaseIntervalsMenuViewModel(this);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			CopyCommand = new RelayCommand(OnCopy, CanEdit);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			ActivateCommand = new RelayCommand(OnActivate, CanActivate);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public virtual void Initialize()
		{
			BuildIntervals();
		}

		private ObservableCollection<TInterval> _intervals;
		public ObservableCollection<TInterval> Intervals
		{
			get { return _intervals; }
			set
			{
				_intervals = value;
				OnPropertyChanged(() => Intervals);
			}
		}

		private TInterval _selectedInterval;
		public TInterval SelectedInterval
		{
			get { return _selectedInterval; }
			set
			{
				if (value == null)
					_selectedInterval = Intervals.First();
				else
				{
					_selectedInterval = value;
					OnPropertyChanged(() => SelectedInterval);
					SelectedInterval.Parts.ForEach(item => item.Update());
					UpdateRibbonItems();
				}
			}
		}

		public void Select(int intervalID)
		{
			if (intervalID >= 0 && intervalID <= 128)
				SelectedInterval = Intervals.First(item => item.Index == intervalID);
			else if (SelectedInterval == null)
				SelectedInterval = Intervals.First();
		}

		public RelayCommand EditCommand { get; private set; }
		protected virtual void OnEdit()
		{
		}
		private bool CanEdit()
		{
			return SelectedInterval != null && SelectedInterval.IsEnabled;
		}

		public RelayCommand ActivateCommand { get; private set; }
		private void OnActivate()
		{
			var isBeforeActive = SelectedInterval.IsActive;
			SelectedInterval.IsActive = !SelectedInterval.IsActive;
			OnPropertyChanged(() => SelectedInterval);
			if (!isBeforeActive)
				EditCommand.Execute();
			UpdateRibbonItems();
		}
		private bool CanActivate()
		{
			return SelectedInterval != null && !SelectedInterval.IsDefault;
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			_copy = CopyInterval(SelectedInterval.Model);
		}

		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			var newInterval = CopyInterval(_copy);
			SelectedInterval.Paste(newInterval);
		}
		private bool CanPaste()
		{
			return _copy != null && SelectedInterval != null && !SelectedInterval.IsDefault;
		}

		public override void OnShow()
		{
			BuildIntervals();
			base.OnShow();
		}
		protected virtual void BuildIntervals()
		{
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
			RibbonItems[0][1].Text = SelectedInterval.ActivateActionTitle;
			RibbonItems[0][1].ImageSource = SelectedInterval.GetActiveImage(!SelectedInterval.IsActive, true);
		}
		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("", ActivateCommand),
					new RibbonMenuItemViewModel("Редактировать", "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("Копировать", CopyCommand, "/Controls;component/Images/BCopy.png"),
					new RibbonMenuItemViewModel("Вставить", PasteCommand, "/Controls;component/Images/BPaste.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 1 }
			};
		}

		private TModel _copy;
		protected virtual TModel CopyInterval(TModel source)
		{
			return source;
		}
	}
}
