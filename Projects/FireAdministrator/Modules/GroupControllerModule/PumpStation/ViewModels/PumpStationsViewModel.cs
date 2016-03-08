using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using GKModule.Events;
using GKModule.Plans;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using KeyboardKey = System.Windows.Input.Key;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class PumpStationsViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public static PumpStationsViewModel Current { get; private set; }
		bool _lockSelection = false;

		public PumpStationsViewModel()
		{
			Current = this;
			Menu = new PumpStationsMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAllEmpty);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			CopyLogicCommand = new RelayCommand(OnCopyLogic, CanCopyLogic);
			PasteLogicCommand = new RelayCommand(OnPasteLogic, CanPasteLogic);
			ShowDependencyItemsCommand = new RelayCommand(ShowDependencyItems);
			RegisterShortcuts();
			IsRightPanelEnabled = true;
			SubscribeEvents();
			SetRibbonItems();
		}
		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.C, ModifierKeys.Control), CopyCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.V, ModifierKeys.Control), PasteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
		}
		protected override bool IsRightPanelVisibleByDefault
		{
			get { return true; }
		}

		public void Initialize()
		{
			PumpStations = new ObservableCollection<PumpStationViewModel>(
				from pumpStation in GKManager.PumpStations
				orderby pumpStation.No
				select new PumpStationViewModel(pumpStation));
			SelectedPumpStation = PumpStations.FirstOrDefault();
		}

		ObservableCollection<PumpStationViewModel> _pumpStations;
		public ObservableCollection<PumpStationViewModel> PumpStations
		{
			get { return _pumpStations; }
			set
			{
				_pumpStations = value;
				OnPropertyChanged(() => PumpStations);
			}
		}

		PumpStationViewModel _selectedPumpStation;
		public PumpStationViewModel SelectedPumpStation
		{
			get { return _selectedPumpStation; }
			set
			{
				_selectedPumpStation = value;
				OnPropertyChanged(() => SelectedPumpStation);
				if (!_lockSelection && _selectedPumpStation != null && _selectedPumpStation.PumpStation.PlanElementUIDs.Count > 0)
					ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(_selectedPumpStation.PumpStation.PlanElementUIDs);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			OnAddResult();
		}
		private PumpStationDetailsViewModel OnAddResult()
		{
			var pumpStationDetailsViewModel = new PumpStationDetailsViewModel();
			if (ServiceFactory.DialogService.ShowModalWindow(pumpStationDetailsViewModel))
			{
				GKManager.AddPumpStation(pumpStationDetailsViewModel.PumpStation);
				var pumpStationViewModel = new PumpStationViewModel(pumpStationDetailsViewModel.PumpStation);
				PumpStations.Add(pumpStationViewModel);
				SelectedPumpStation = pumpStationViewModel;
				ServiceFactory.SaveService.GKChanged = true;
				GKPlanExtension.Instance.Cache.BuildSafe<GKPumpStation>();
				return pumpStationDetailsViewModel;
			}
			return null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			OnEdit(SelectedPumpStation.PumpStation);
		}
		void OnEdit(GKPumpStation pumpStation)
		{
			var pumpStationDetailsViewModel = new PumpStationDetailsViewModel(pumpStation);
			if (ServiceFactory.DialogService.ShowModalWindow(pumpStationDetailsViewModel))
			{
				SelectedPumpStation.Update();
				GKManager.EditPumpStation(SelectedPumpStation.PumpStation);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (ServiceFactory.MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить направление " + SelectedPumpStation.PumpStation.PresentationName + " ?"))
			{
				var index = PumpStations.IndexOf(SelectedPumpStation);
				GKManager.RemovePumpStation(SelectedPumpStation.PumpStation);
				PumpStations.Remove(SelectedPumpStation);
				index = Math.Min(index, PumpStations.Count - 1);
				if (index > -1)
					SelectedPumpStation = PumpStations[index];
				ServiceFactory.SaveService.GKChanged = true;
				GKPlanExtension.Instance.Cache.BuildSafe<GKPumpStation>();
			}
		}

		public RelayCommand DeleteAllEmptyCommand { get; private set; }
		void OnDeleteAllEmpty()
		{
			if (ServiceFactory.MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить все пустые направления ?"))
			{
				GetEmptyPumpStations().ForEach(x =>
				{
					GKManager.RemovePumpStation(x.PumpStation);
					PumpStations.Remove(x);
				});

				SelectedPumpStation = PumpStations.FirstOrDefault();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		bool CanDeleteAllEmpty()
		{
			return GetEmptyPumpStations().Any();
		}
		List<PumpStationViewModel> GetEmptyPumpStations()
		{
			return PumpStations.Where(x => !x.PumpStation.StartLogic.GetObjects().Any()).ToList();
		}

		GKPumpStation _pumpStationToCopy;
		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			_pumpStationToCopy = SelectedPumpStation.PumpStation.Clone();
			var logicViewModel = new LogicViewModel(SelectedPumpStation.PumpStation, SelectedPumpStation.PumpStation.StartLogic, true);
			_pumpStationToCopy.StartLogic = logicViewModel.GetModel();
		}

		bool CanCopy()
		{
			return SelectedPumpStation != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			_pumpStationToCopy.UID = Guid.NewGuid();
			var pumpStationViewModel = new PumpStationViewModel(_pumpStationToCopy.Clone());
			var logicViewModel = new LogicViewModel(SelectedPumpStation.PumpStation, _pumpStationToCopy.StartLogic, true);
			pumpStationViewModel.PumpStation.StartLogic = logicViewModel.GetModel();
			pumpStationViewModel.PumpStation.No = (ushort)(GKManager.PumpStations.Select(x => x.No).Max() + 1);
			pumpStationViewModel.PumpStation.Invalidate(GKManager.DeviceConfiguration);
			GKManager.AddPumpStation(pumpStationViewModel.PumpStation);
			PumpStations.Add(pumpStationViewModel);
			SelectedPumpStation = pumpStationViewModel;
			ServiceFactory.SaveService.GKChanged = true;
		}

		bool CanPaste()
		{
			return _pumpStationToCopy != null && SelectedPumpStation != null;
		}

		public RelayCommand CopyLogicCommand { get; private set; }
		void OnCopyLogic()
		{
			GKManager.CopyLogic(SelectedPumpStation.PumpStation.StartLogic, true, false, true, false, true);
		}

		bool CanCopyLogic()
		{
			return SelectedPumpStation != null;
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
				SelectedPumpStation.PumpStation.StartLogic = GKManager.PasteLogic(new GKAdvancedLogic(true, false, true, false, true));
				SelectedPumpStation.PumpStation.Invalidate(GKManager.DeviceConfiguration);
				SelectedPumpStation.Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		bool CanPasteLogic()
		{
			return SelectedPumpStation != null && GKManager.LogicToCopy != null;
		}

		bool CanEditDelete()
		{
			return SelectedPumpStation != null;
		}

		public RelayCommand ShowDependencyItemsCommand { get; set; }

		void ShowDependencyItems()
		{
			if (SelectedPumpStation != null)
			{
				var dependencyItemsViewModel = new DependencyItemsViewModel(SelectedPumpStation.PumpStation.OutputDependentElements);
				DialogService.ShowModalWindow(dependencyItemsViewModel);
			}
		}

		public void CreatePumpStation(CreateGKPumpStationEventArgs createPumpStationEventArg)
		{
			PumpStationDetailsViewModel result = OnAddResult();
			if (result == null)
			{
				createPumpStationEventArg.Cancel = true;
				createPumpStationEventArg.PumpStationUID = Guid.Empty;
			}
			else
			{
				createPumpStationEventArg.Cancel = false;
				createPumpStationEventArg.PumpStationUID = result.PumpStation.UID;
				createPumpStationEventArg.PumpStation = result.PumpStation;
			}
		}
		public void EditPumpStation(Guid pumpStationUID)
		{
			var pumpStationViewModel = pumpStationUID == Guid.Empty ? null : PumpStations.FirstOrDefault(x => x.PumpStation.UID == pumpStationUID);
			if (pumpStationViewModel != null)
				OnEdit(pumpStationViewModel.PumpStation);
		}

		private void OnPumpStationChanged(Guid pumpStationUID)
		{
			var pumpStation = PumpStations.FirstOrDefault(x => x.PumpStation.UID == pumpStationUID);
			if (pumpStation != null)
			{
				pumpStation.Update();
				// TODO: FIX IT
				if (!_lockSelection)
					SelectedPumpStation = pumpStation;
			}
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			Guid guid = Guid.Empty;
			_lockSelection = true;
			elements.ForEach(element =>
			{
				var elementPumpStation = GetElementPumpStation(element);
				if (elementPumpStation != null)
				{
					OnPumpStationChanged(elementPumpStation.PumpStationUID);
				}
			});
			_lockSelection = false;
		}
		private void OnElementSelected(ElementBase element)
		{
			var elementPumpStation = GetElementPumpStation(element);
			if (elementPumpStation != null)
			{
				_lockSelection = true;
				Select(elementPumpStation.PumpStationUID);
				_lockSelection = false;
			}
		}
		private IElementPumpStation GetElementPumpStation(ElementBase element)
		{
			IElementPumpStation elementPumpStation = element as ElementRectangleGKPumpStation;
			if (elementPumpStation == null)
				elementPumpStation = element as ElementPolygonGKPumpStation;
			return elementPumpStation;
		}

		public void LockedSelect(Guid zoneUID)
		{
			_lockSelection = true;
			Select(zoneUID);
			_lockSelection = false;
		}

		public override void OnShow()
		{
			base.OnShow();
			SelectedPumpStation = SelectedPumpStation;
		}

		#region ISelectable<Guid> Members
		public void Select(Guid pumpStationUID)
		{
			if (pumpStationUID != Guid.Empty)
				SelectedPumpStation = PumpStations.FirstOrDefault(x => x.PumpStation.UID == pumpStationUID);
		}
		#endregion

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
					new RibbonMenuItemViewModel("Удалить все пустые направления", DeleteAllEmptyCommand, "BDeleteEmpty"),
				}, "BEdit") { Order = 2 }
			};
		}
	}
}