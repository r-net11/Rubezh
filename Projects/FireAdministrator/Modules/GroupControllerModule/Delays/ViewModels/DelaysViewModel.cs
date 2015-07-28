using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common;
using FiresecAPI.GK;
using FiresecClient;
using GKModule.Events;
using GKModule.Plans;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

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
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			CopyLogicCommand = new RelayCommand(OnCopyLogic, CanCopyLogic);
			PasteLogicCommand = new RelayCommand(OnPasteLogic, CanPasteLogic);

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

		GKDelay _delayToCopy;
		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			_delayToCopy = Utils.Clone(SelectedDelay.Delay);
			var logicViewModel = new LogicViewModel(SelectedDelay.Delay, SelectedDelay.Delay.Logic, true);
			_delayToCopy.Logic = logicViewModel.GetModel();
		}

		bool CanCopy()
		{
			return SelectedDelay != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			_delayToCopy.UID = Guid.NewGuid();
			var delayViewModel = new DelayViewModel(Utils.Clone(_delayToCopy));
			var logicViewModel = new LogicViewModel(SelectedDelay.Delay, _delayToCopy.Logic, true);
			delayViewModel.Delay.Logic = logicViewModel.GetModel();
			delayViewModel.Delay.No = (ushort)(GKManager.Delays.Select(x => x.No).Max() + 1);
			GKManager.Delays.Add(delayViewModel.Delay);
			Delays.Add(delayViewModel);
			SelectedDelay = delayViewModel;
			ServiceFactory.SaveService.AutomationChanged = true;
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

		bool CanCopyLogic()
		{
			return SelectedDelay != null;
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
			}
		}

		bool CanPasteLogic()
		{
			return SelectedDelay != null && GKManager.LogicToCopy != null;
		}

		public bool HasSelectedDelay
		{
			get { return SelectedDelay != null; }
		}

		bool CanEditDelete()
		{
			return SelectedDelay != null;
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

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить задержку " + SelectedDelay.Delay.Name))
			{
				var index = Delays.IndexOf(SelectedDelay);
				GKManager.Delays.Remove(SelectedDelay.Delay);
				Delays.Remove(SelectedDelay);
				index = Math.Min(index, Delays.Count - 1);
				if (index > -1)
					SelectedDelay = Delays[index];
				OnPropertyChanged(() => HasSelectedDelay);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			this.OnEdit(this.SelectedDelay.Delay);
		}

		/// <summary>
		/// Handles editing a Delay.
		/// </summary>
		/// <param name="delay">Delay to be edited.</param>
		private void OnEdit(GKDelay delay)
		{
			var delayDetailsViewModel = new DelayDetailsViewModel(delay);
			if (DialogService.ShowModalWindow(delayDetailsViewModel))
			{
				SelectedDelay.Delay = delayDetailsViewModel.Delay;
				SelectedDelay.Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
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

		public override void OnHide()
		{
			base.OnHide();
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.C, ModifierKeys.Control), CopyCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.V, ModifierKeys.Control), PasteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}

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
				}, "BEdit") { Order = 2 }
			};
		}

		#region Fields
		private bool _lockSelection = false;

		#endregion
	}
}