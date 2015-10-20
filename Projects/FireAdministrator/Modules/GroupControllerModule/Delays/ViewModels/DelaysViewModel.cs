using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common;
using RubezhAPI.GK;
using RubezhClient;
using GKModule.Events;
using GKModule.Plans;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using Infrastructure.Common.Services;
using GKModule.ViewModels;

namespace GKModule.ViewModels
{
	public class DelaysViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public static DelaysViewModel Current { get; private set; }

		public DelaysViewModel()
		{
			Current = this;
			Menu = new DelaysMenuViewModel(this);
			AddCommand = new RelayCommand(() => OnAdd());
			EditCommand = new RelayCommand(() => OnEdit(SelectedDelay.Delay), () => HasSelectedDelay);
			DeleteCommand = new RelayCommand(OnDelete, () => HasSelectedDelay);
			DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAllEmpty);
			CopyCommand = new RelayCommand(OnCopy, () => HasSelectedDelay);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			CopyLogicCommand = new RelayCommand(OnCopyLogic, () => HasSelectedDelay);
			PasteLogicCommand = new RelayCommand(OnPasteLogic, CanPasteLogic);
			ShowDependencyItemsCommand = new RelayCommand(ShowDependencyItems);

			IsRightPanelEnabled = true;
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Delays = GKManager.Delays == null ? new ObservableCollection<DelayViewModel>() : new ObservableCollection<DelayViewModel>(
				from delay in GKManager.Delays
				orderby delay.No
				select new DelayViewModel(delay));
			SelectedDelay = Delays.FirstOrDefault();
		}

		ObservableCollection<DelayViewModel> _delays;
		public ObservableCollection<DelayViewModel> Delays
		{
			get { return _delays; }
			set
			{
				_delays = value;
				OnPropertyChanged(() => Delays);
			}
		}

		DelayViewModel _selectedDelay;
		public DelayViewModel SelectedDelay
		{
			get { return _selectedDelay; }
			set
			{
				_selectedDelay = value;
				OnPropertyChanged(() => SelectedDelay);
			}
		}

		public bool HasSelectedDelay
		{
			get { return SelectedDelay != null; }
		}

		public RelayCommand AddCommand { get; private set; }
		private DelayDetailsViewModel OnAdd()
		{
			var delayDetailsViewModel = new DelayDetailsViewModel();
			if (DialogService.ShowModalWindow(delayDetailsViewModel))
			{
				GKManager.AddDelay(delayDetailsViewModel.Delay);
				var delayViewModel = new DelayViewModel(delayDetailsViewModel.Delay);
				Delays.Add(delayViewModel);
				SelectedDelay = delayViewModel;
				ServiceFactory.SaveService.GKChanged = true;
				GKPlanExtension.Instance.Cache.BuildSafe<GKDelay>();
				return delayDetailsViewModel;
			}
			return null;
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit(GKDelay delay)
		{
			var delayDetailsViewModel = new DelayDetailsViewModel(delay);
			if (DialogService.ShowModalWindow(delayDetailsViewModel))
			{
				GKManager.EditDelay(SelectedDelay.Delay);
				SelectedDelay.Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить задержку " + SelectedDelay.Delay.Name))
			{
				var index = Delays.IndexOf(SelectedDelay);
				GKManager.RemoveDelay(SelectedDelay.Delay);
				Delays.Remove(SelectedDelay);
				index = Math.Min(index, Delays.Count - 1);
				if (index > -1)
					SelectedDelay = Delays[index];
				OnPropertyChanged(() => HasSelectedDelay);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteAllEmptyCommand { get; private set; }
		void OnDeleteAllEmpty()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить все пустые задержки ?"))
			{
				var emptyDelays = Delays.Where(x => !x.Delay.Logic.GetObjects().Any()).ToList();

				if (emptyDelays.Any())
				{
					for (var i = emptyDelays.Count() - 1; i >= 0; i--)
					{
						GKManager.RemoveDelay(emptyDelays.ElementAt(i).Delay);
						Delays.Remove(emptyDelays.ElementAt(i));
					}

					SelectedDelay = Delays.FirstOrDefault();
					ServiceFactory.SaveService.GKChanged = true;
				}
			}
		}

		bool CanDeleteAllEmpty()
		{
			return Delays.Any(x => !x.Delay.Logic.GetObjects().Any());
		}

		GKDelay _delayToCopy;
		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			_delayToCopy = SelectedDelay.Delay.Clone();
			var logicViewModel = new LogicViewModel(SelectedDelay.Delay, SelectedDelay.Delay.Logic, true);
			_delayToCopy.Logic = logicViewModel.GetModel();
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var logicViewModel = new LogicViewModel(SelectedDelay.Delay, _delayToCopy.Logic, true);
			_delayToCopy.UID = Guid.NewGuid();
			var delay = _delayToCopy.Clone();
			delay.Logic = logicViewModel.GetModel();
			delay.No = (ushort)(GKManager.Delays.Select(x => x.No).Max() + 1);
			delay.Invalidate();
			var delayViewModel = new DelayViewModel(delay);
			GKManager.Delays.Add(delayViewModel.Delay);
			Delays.Add(delayViewModel);
			SelectedDelay = delayViewModel;
			ServiceFactory.SaveService.GKChanged = true;
		}

		bool CanPaste()
		{
			return _delayToCopy != null;
		}

		public RelayCommand CopyLogicCommand { get; private set; }
		void OnCopyLogic()
		{
			GKManager.CopyLogic(SelectedDelay.Delay.Logic, true, false, true, false, true);
		}

		public RelayCommand PasteLogicCommand { get; private set; }
		void OnPasteLogic()
		{
			var result = GKManager.CompareLogic(new GKAdvancedLogic(true, false, true, false, true));
			var messageBoxResult = true;
			if (!String.IsNullOrEmpty(result))
				messageBoxResult = MessageBoxService.ShowConfirmation(result, "Копировать логику?");
			if (messageBoxResult)
			{
				SelectedDelay.Delay.Logic = GKManager.PasteLogic(new GKAdvancedLogic(true, false, true, false, true));
				SelectedDelay.Update();
				SelectedDelay.Delay.Invalidate();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		bool CanPasteLogic()
		{
			return SelectedDelay != null && GKManager.LogicToCopy != null;
		}

		public RelayCommand ShowDependencyItemsCommand { get; set; }

		void ShowDependencyItems()
		{
			if (SelectedDelay != null)
			{
				var dependencyItemsViewModel = new DependencyItemsViewModel(SelectedDelay.Delay.OutDependentElements);
				DialogService.ShowModalWindow(dependencyItemsViewModel);
			}
		}

		/// <summary>
		/// Creates new Delay.
		/// </summary>
		/// <param name="createDelayEventArg">Argument for Delay Creation.</param>
		public void CreateDelay(CreateGKDelayEventArgs createDelayEventArg)
		{
			DelayDetailsViewModel result = this.OnAdd();
			if (result == null)
			{
				createDelayEventArg.Cancel = true;
				createDelayEventArg.DelayUID = Guid.Empty;
			}
			else
			{
				createDelayEventArg.Cancel = false;
				createDelayEventArg.DelayUID = result.Delay.UID;
				createDelayEventArg.Delay = result.Delay;
			}
		}

		/// <summary>
		/// Edits specified Delay.
		/// </summary>
		/// <param name="delayUID">UID of the Delay to edit.</param>
		public void EditDelay(Guid delayUID)
		{
			var delayViewModel = delayUID == Guid.Empty ? null : this.Delays.FirstOrDefault(x => x.Delay.UID == delayUID);
			if (delayViewModel != null)
				this.OnEdit(delayViewModel.Delay);
		}

		public void Select(Guid delayUID)
		{
			if (delayUID != Guid.Empty)
				SelectedDelay = Delays.FirstOrDefault(x => x.Delay.UID == delayUID);
		}

		public override void OnShow()
		{
			base.OnShow();
			SelectedDelay = SelectedDelay;
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.C, ModifierKeys.Control), CopyCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.V, ModifierKeys.Control), PasteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}

		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", AddCommand, "BAdd"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "BEdit"),
					new RibbonMenuItemViewModel("Копировать", CopyCommand, "BCopy"),
					new RibbonMenuItemViewModel("Вставить", PasteCommand, "BPaste"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "BDelete"),
					new RibbonMenuItemViewModel("Удалить все пустые задержки", DeleteAllEmptyCommand, "BDeleteEmpty"),
				}, "BEdit") { Order = 2 }
			};
		}

		private bool _lockSelection = false;
		public void LockedSelect(Guid zoneUID)
		{
			try
			{
				this._lockSelection = true;
				Select(zoneUID);
			}
			finally
			{
				this._lockSelection = false;
			}
		}
	}
}