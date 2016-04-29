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
	public class PumpStationsViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public static PumpStationsViewModel Current { get; private set; }
		bool _lockSelection = false;

		public PumpStationsViewModel()
		{
			Current = this;
			Menu = new PumpStationsMenuViewModel(this);
			AddCommand = new RelayCommand(() => OnAdd());
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			ChangePumpDevicesCommand = new RelayCommand(OnChangePumpDevices, CanChangePumpDevices);
			DeletePumpDeviceCommand = new RelayCommand(OnDeletePumpDevice, CanDeletePumpDevice);
			DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAllEmpty);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			CopyLogicCommand = new RelayCommand(OnCopyLogic, CanCopyLogic);
			PasteLogicCommand = new RelayCommand(OnPasteLogic, CanPasteLogic);
			ShowDependencyItemsCommand = new RelayCommand(ShowDependencyItems);

			Menu = new PumpStationsMenuViewModel(this);
			RegisterShortcuts();
			IsRightPanelEnabled = true;
			SetRibbonItems();

			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
		}
		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
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
			PumpStations = GKManager.PumpStations == null ? new ObservableCollection<PumpStationViewModel>() : new ObservableCollection<PumpStationViewModel>(
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
				OnPropertyChanged("PumpStations");
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

		public bool HasSelectedPumpStation
		{
			get { return SelectedPumpStation != null; }
		}

		public RelayCommand AddCommand { get; private set; }
		private PumpStationDetailsViewModel OnAdd()
		{
			var pumpStationDetailsViewModel = new PumpStationDetailsViewModel();
			if (ServiceFactory.DialogService.ShowModalWindow(pumpStationDetailsViewModel))
			{
				GKManager.AddPumpStation(pumpStationDetailsViewModel.PumpStation);
				var pumpStationViewModel = new PumpStationViewModel(pumpStationDetailsViewModel.PumpStation);
				PumpStations.Add(pumpStationViewModel);
				SelectedPumpStation = pumpStationViewModel;
				OnPropertyChanged(() => HasSelectedPumpStation);
				ServiceFactory.SaveService.GKChanged = true;
				GKPlanExtension.Instance.Cache.BuildSafe<GKPumpStation>();
				return pumpStationDetailsViewModel;
			}
			return null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			OnEdit(SelectedPumpStation);
		}
		void OnEdit(PumpStationViewModel pumpStationViewModel)
		{
			var pumpStationDetailsViewModel = new PumpStationDetailsViewModel(pumpStationViewModel.PumpStation);
			if (ServiceFactory.DialogService.ShowModalWindow(pumpStationDetailsViewModel))
			{
				pumpStationViewModel.Update();
				GKManager.EditPumpStation(pumpStationViewModel.PumpStation);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (ServiceFactory.MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить НС " + SelectedPumpStation.PumpStation.PresentationName + " ?"))
			{
				var pumpDevices = new List<GKDevice>(SelectedPumpStation.PumpDevices.Select(x => x.Device));
				var index = PumpStations.IndexOf(SelectedPumpStation);
				GKManager.RemovePumpStation(SelectedPumpStation.PumpStation);
				PumpStations.Remove(SelectedPumpStation);
				index = Math.Min(index, PumpStations.Count - 1);
				if (index > -1)
					SelectedPumpStation = PumpStations[index];
				OnPropertyChanged(() => HasSelectedPumpStation);
				ServiceFactory.SaveService.GKChanged = true;
				GKPlanExtension.Instance.Cache.BuildSafe<GKPumpStation>();
			}
		}

		public RelayCommand DeleteAllEmptyCommand { get; private set; }
		void OnDeleteAllEmpty()
		{
			if (ServiceFactory.MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить все пустые НС ?"))
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
			return PumpStations.Where(x => !x.PumpStation.InputDependentElements.Any()).ToList();
		}

		public RelayCommand ChangePumpDevicesCommand { get; private set; }
		void OnChangePumpDevices()
		{
			SelectedPumpStation.ChangePumpDevices();
		}
		bool CanChangePumpDevices()
		{
			return SelectedPumpStation != null;
		}

		public RelayCommand DeletePumpDeviceCommand { get; private set; }
		void OnDeletePumpDevice()
		{
			SelectedPumpStation.DeletePumpDevice();
		}
		bool CanDeletePumpDevice()
		{
			return SelectedPumpStation != null && SelectedPumpStation.SelectedPumpDevice != null;
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
			PumpStationDetailsViewModel result = OnAdd();
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
				OnEdit(pumpStationViewModel);
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
					new RibbonMenuItemViewModel("Удалить все пустые НС", DeleteAllEmptyCommand, "BDeleteEmpty"),
				}, "BEdit") { Order = 2 }
			};
		}
	}
}