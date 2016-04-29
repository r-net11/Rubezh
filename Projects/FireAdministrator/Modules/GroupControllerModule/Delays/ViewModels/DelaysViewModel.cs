using GKModule.Events;
using GKModule.Plans;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans.Events;
using Infrastructure.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;

namespace GKModule.ViewModels
{
	public class DelaysViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public static DelaysViewModel Current { get; private set; }

		public DelaysViewModel()
		{
			Current = this;
			Menu = new DelaysMenuViewModel(this);
			AddCommand = new RelayCommand(() => OnAdd());
			EditCommand = new RelayCommand(() => OnEdit(SelectedDelay), () => HasSelectedDelay);
			DeleteCommand = new RelayCommand(OnDelete, () => HasSelectedDelay);
			DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAllEmpty);
			CopyCommand = new RelayCommand(OnCopy, () => HasSelectedDelay);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			CopyLogicCommand = new RelayCommand(OnCopyLogic, () => HasSelectedDelay);
			PasteLogicCommand = new RelayCommand(OnPasteLogic, CanPasteLogic);
			ShowDependencyItemsCommand = new RelayCommand(ShowDependencyItems);

			IsRightPanelEnabled = true;
			SetRibbonItems();
			RegisterShortcuts();
			SubscribeEvents();
		}
		public void Initialize()
		{
			Delays = GKManager.Delays == null ? new ObservableCollection<DelayViewModel>() : new ObservableCollection<DelayViewModel>(GKManager.Delays.OrderBy(x => x.No).Select(x => new DelayViewModel(x)));
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
				if (value != null)
					value.Update();
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
			if (ServiceFactory.DialogService.ShowModalWindow(delayDetailsViewModel))
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
		private void OnEdit(DelayViewModel delayViewModel)
		{
			var delayDetailsViewModel = new DelayDetailsViewModel(delayViewModel.Delay);
			if (ServiceFactory.DialogService.ShowModalWindow(delayDetailsViewModel))
			{
				GKManager.EditDelay(delayViewModel.Delay);
				delayViewModel.Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (ServiceFactory.MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить задержку " + SelectedDelay.Delay.PresentationName + " ?"))
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
			if (ServiceFactory.MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить все пустые задержки ?"))
			{
				GetEmptyDelays().ForEach(x =>
				{
					GKManager.RemoveDelay(x.Delay);
					Delays.Remove(x);
				});

				SelectedDelay = Delays.FirstOrDefault();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		bool CanDeleteAllEmpty()
		{
			return GetEmptyDelays().Any();
		}

		List<DelayViewModel> GetEmptyDelays()
		{
			return Delays.Where(x => !x.Delay.Logic.GetObjects().Any()).ToList();
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
			delay.Invalidate(GKManager.DeviceConfiguration);
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
			GKManager.CopyLogic(SelectedDelay.Delay.Logic, true, false, true);
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
				SelectedDelay.Delay.Invalidate(GKManager.DeviceConfiguration);
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
				var dependencyItemsViewModel = new DependencyItemsViewModel(SelectedDelay.Delay.OutputDependentElements);
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
				this.OnEdit(delayViewModel);
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

		private bool _lockSelection;
		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.C, ModifierKeys.Control), CopyCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.V, ModifierKeys.Control), PasteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
		}
		void SubscribeEvents()
		{
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
		}
		void OnElementChanged(List<ElementBase> elements)
		{
			elements.ForEach(element =>
			{
				var elementDelay = GetElementGKDelay(element);
				if (elementDelay != null)
					OnDelayChanged(elementDelay.DelayUID);
			});
		}
		void OnDelayChanged(Guid delayUID)
		{
			var delay = Delays.FirstOrDefault(x => x.Delay.UID == delayUID);
			if (delay != null)
				delay.Update();
		}
		void OnElementSelected(ElementBase element)
		{
			var elementDelay = GetElementGKDelay(element);
			if (elementDelay != null)
			{
				_lockSelection = true;
				Select(elementDelay.DelayUID);
				_lockSelection = false;
			}
		}
		IElementDelay GetElementGKDelay(ElementBase element)
		{
			IElementDelay elementDelay = element as ElementRectangleGKDelay;
			if (elementDelay == null)
				elementDelay = element as ElementPolygonGKDelay;
			return elementDelay;
		}
	}
}