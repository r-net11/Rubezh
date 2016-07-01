using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Localization.Strazh.Common;
using Localization.Strazh.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using Common;

namespace StrazhModule.Intervals.Base.ViewModels
{
	public class BaseIntervalsViewModel<TInterval, TPart, TModel> : MenuViewPartViewModel, ISelectable<int>, IEditingBaseViewModel
		where TInterval : BaseIntervalViewModel<TPart, TModel>
		where TPart : BaseIntervalPartViewModel
	{
		public BaseIntervalsViewModel()
		{
			Menu = new BaseIntervalsMenuViewModel(this);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
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
					_selectedInterval = Intervals.FirstOrDefault();
				else
				{
					_selectedInterval = value;
					OnPropertyChanged(() => SelectedInterval);
					SelectedInterval.Parts.ForEach(item => item.Update());
					UpdateRibbonItems();
				}
			}
		}

		#region <Реализация ISelecteble<int>>

		public void Select(int intervalID)
		{
			if (intervalID >= 0 && intervalID <= 128)
				SelectedInterval = Intervals.FirstOrDefault(item => item.Index == intervalID);
			else if (SelectedInterval == null)
				SelectedInterval = Intervals.FirstOrDefault();
		}

		#endregion </Реализация ISelecteble<int>>

		#region <Реализация IEditingBaseViewModel>

		public RelayCommand EditCommand { get; private set; }

		#endregion </Реализация IEditingBaseViewModel>

		protected virtual void OnEdit()
		{
		}
		private bool CanEdit()
		{
			return SelectedInterval != null && SelectedInterval.IsActive;
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
			return SelectedInterval != null && !SelectedInterval.IsPredefined;
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			_copy = CopyInterval(SelectedInterval.Model);
		}
		private bool CanCopy()
		{
			return SelectedInterval != null && (SelectedInterval.IsPredefined || SelectedInterval.IsActive);
		}

		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			var newInterval = CopyInterval(_copy);
			SelectedInterval.Paste(newInterval);
		}
		private bool CanPaste()
		{
			return _copy != null && SelectedInterval != null && !SelectedInterval.IsPredefined;
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
				new RibbonMenuItemViewModel(CommonViewModels.Edition, new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("", ActivateCommand),
					new RibbonMenuItemViewModel(CommonResources.Edit, "BEdit"),
					new RibbonMenuItemViewModel(CommonResources.Copy, CopyCommand, "BCopy"),
					new RibbonMenuItemViewModel(CommonResources.Paste, PasteCommand, "BPaste"),
				}, "BEdit") { Order = 1 }
			};
		}

		private TModel _copy;
		protected virtual TModel CopyInterval(TModel source)
		{
			return source;
		}
	}
}