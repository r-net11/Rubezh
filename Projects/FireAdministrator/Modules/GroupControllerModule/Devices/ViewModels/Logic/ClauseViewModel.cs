using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class ClauseViewModel : BaseViewModel
	{
		public List<GKDevice> Devices { get; set; }
		public List<GKZone> Zones { get; set; }
		public List<GKGuardZone> GuardZones { get; set; }
		public List<GKDirection> Directions { get; set; }
		public List<GKMPT> MPTs { get; set; }
		public List<GKDelay> Delays { get; set; }
		public List<GKDoor> Doors { get; set; }
		public List<GKPumpStation> PumpStations { get; set; }
		GKBase GKBase { get; set; }

		public ClauseViewModel(GKBase gkBase, GKClause clause)
		{
			SelectDevicesCommand = new RelayCommand(OnSelectDevices);
			SelectZonesCommand = new RelayCommand(OnSelectZones);
			SelectGuardZonesCommand = new RelayCommand(OnSelectGuardZones);
			SelectDirectionCommand = new RelayCommand(OnSelectDirections);
			SelectMPTsCommand = new RelayCommand(OnSelectMPTs);
			SelectDelaysCommand = new RelayCommand(OnSelectDelays);
			SelectDoorsCommand = new RelayCommand(OnSelectDoors);
			SelectPumpStationsCommand = new RelayCommand(OnSelectPumpStations);

			GKBase = gkBase;
			ClauseConditionTypes = Enum.GetValues(typeof(ClauseConditionType)).Cast<ClauseConditionType>().ToList();
			ClauseOperationTypes = Enum.GetValues(typeof(ClauseOperationType)).Cast<ClauseOperationType>().ToList();
			SelectedClauseOperationType = clause.ClauseOperationType;

			Devices = clause.Devices.ToList();
			Zones = clause.Zones.ToList();
			GuardZones = clause.GuardZones.ToList();
			Directions = clause.Directions.ToList();
			MPTs = clause.MPTs.ToList();
			Delays = clause.Delays.ToList();
			Doors = clause.Doors.ToList();
			PumpStations = clause.PumpStations.ToList();

			SelectedClauseConditionType = clause.ClauseConditionType;
			SelectedStateType = StateTypes.FirstOrDefault(x => x.StateBit == clause.StateType);
		}

		public List<ClauseConditionType> ClauseConditionTypes { get; private set; }
		public List<ClauseOperationType> ClauseOperationTypes { get; private set; }

		ClauseConditionType _selectedClauseConditionType;
		public ClauseConditionType SelectedClauseConditionType
		{
			get { return _selectedClauseConditionType; }
			set
			{
				_selectedClauseConditionType = value;
				OnPropertyChanged(() => SelectedClauseConditionType);
			}
		}

		ClauseOperationType _selectedClauseOperationType;
		public ClauseOperationType SelectedClauseOperationType
		{
			get { return _selectedClauseOperationType; }
			set
			{
				_selectedClauseOperationType = value;
				var oldSelectedStateType = SelectedStateType != null ? SelectedStateType.StateBit : GKStateBit.Test;

				switch (value)
				{
					case ClauseOperationType.AllDevices:
					case ClauseOperationType.AnyDevice:
						StateTypes = new ObservableCollection<StateTypeViewModel>()
						{
							new StateTypeViewModel(value, GKStateBit.Norm),
							new StateTypeViewModel(value, GKStateBit.Fire2),
							new StateTypeViewModel(value, GKStateBit.Fire1),
							new StateTypeViewModel(value, GKStateBit.On),
							new StateTypeViewModel(value, GKStateBit.Off),
							new StateTypeViewModel(value, GKStateBit.TurningOn),
							new StateTypeViewModel(value, GKStateBit.TurningOff),
							new StateTypeViewModel(value, GKStateBit.Failure),
						};
						break;

					case ClauseOperationType.AllZones:
					case ClauseOperationType.AnyZone:
						StateTypes = new ObservableCollection<StateTypeViewModel>()
						{
							new StateTypeViewModel(value, GKStateBit.Fire2),
							new StateTypeViewModel(value, GKStateBit.Fire1),
							new StateTypeViewModel(value, GKStateBit.Attention)
						};
						break;

					case ClauseOperationType.AllGuardZones:
					case ClauseOperationType.AnyGuardZone:
						StateTypes = new ObservableCollection<StateTypeViewModel>()
						{
							new StateTypeViewModel(value, GKStateBit.On),
							new StateTypeViewModel(value, GKStateBit.Off),
							new StateTypeViewModel(value, GKStateBit.Fire1)
						};
						break;

					case ClauseOperationType.AllDirections:
					case ClauseOperationType.AnyDirection:
						StateTypes = new ObservableCollection<StateTypeViewModel>()
						{
							new StateTypeViewModel(value, GKStateBit.On),
							new StateTypeViewModel(value, GKStateBit.Off),
							new StateTypeViewModel(value, GKStateBit.TurningOn),
							new StateTypeViewModel(value, GKStateBit.Norm)
						};
						break;

					case ClauseOperationType.AllMPTs:
					case ClauseOperationType.AnyMPT:
						StateTypes = new ObservableCollection<StateTypeViewModel>()
						{
							new StateTypeViewModel(value, GKStateBit.On),
							new StateTypeViewModel(value, GKStateBit.Off),
							new StateTypeViewModel(value, GKStateBit.TurningOn),
							new StateTypeViewModel(value, GKStateBit.Norm)
						};
						break;

					case ClauseOperationType.AllDelays:
					case ClauseOperationType.AnyDelay:
						StateTypes = new ObservableCollection<StateTypeViewModel>()
						{
							new StateTypeViewModel(value, GKStateBit.On),
							new StateTypeViewModel(value, GKStateBit.Off),
							new StateTypeViewModel(value, GKStateBit.TurningOn),
							new StateTypeViewModel(value, GKStateBit.Norm)
						};
						break;

					case ClauseOperationType.AnyDoor:
					case ClauseOperationType.AllDoors:
						StateTypes = new ObservableCollection<StateTypeViewModel>()
						{
							new StateTypeViewModel(value, GKStateBit.On),
							new StateTypeViewModel(value, GKStateBit.Fire1)
						};
						break;

					case ClauseOperationType.AnyPumpStation:
					case ClauseOperationType.AllPumpStations:
						StateTypes = new ObservableCollection<StateTypeViewModel>()
						{
							new StateTypeViewModel(value, GKStateBit.On),
							new StateTypeViewModel(value, GKStateBit.TurningOn),
							new StateTypeViewModel(value, GKStateBit.Norm)
						};
						break;
				}
				if (StateTypes.Any(x => x.StateBit == oldSelectedStateType))
				{
					SelectedStateType = StateTypes.FirstOrDefault(x => x.StateBit == oldSelectedStateType);
				}
				if (SelectedStateType == null)
				{
					SelectedStateType = StateTypes.FirstOrDefault();
				}
				OnPropertyChanged(() => SelectedClauseOperationType);
				OnPropertyChanged(() => PresenrationDevices);
				OnPropertyChanged(() => PresenrationZones);
				OnPropertyChanged(() => PresenrationGuardZones);
				OnPropertyChanged(() => PresenrationDirections);
				OnPropertyChanged(() => PresenrationMPTs);
				OnPropertyChanged(() => PresenrationDelays);
				OnPropertyChanged(() => PresenrationDoors);
				OnPropertyChanged(() => CanSelectDevices);
				OnPropertyChanged(() => CanSelectZones);
				OnPropertyChanged(() => CanSelectGuardZones);
				OnPropertyChanged(() => CanSelectDirections);
				OnPropertyChanged(() => CanSelectMPTs);
				OnPropertyChanged(() => CanSelectDelays);
				OnPropertyChanged(() => CanSelectDoors);
				OnPropertyChanged(() => CanSelectPumpStations);
			}
		}

		ObservableCollection<StateTypeViewModel> _stateTypes;
		public ObservableCollection<StateTypeViewModel> StateTypes
		{
			get { return _stateTypes; }
			set
			{
				_stateTypes = value;
				OnPropertyChanged(() => StateTypes);
			}
		}

		StateTypeViewModel _selectedStateType;
		public StateTypeViewModel SelectedStateType
		{
			get { return _selectedStateType; }
			set
			{
				_selectedStateType = value;
				if (value!= null && Devices != null && (SelectedClauseOperationType == ClauseOperationType.AllDevices || SelectedClauseOperationType == ClauseOperationType.AnyDevice))
				{
					Devices.RemoveAll(x => !x.Driver.AvailableStateBits.Contains(value.StateBit));
					OnPropertyChanged(() => PresenrationDevices);
				}
				OnPropertyChanged(() => SelectedStateType);
			}
		}

		public string PresenrationDevices
		{
			get { return GKManager.GetCommaSeparatedDevices(Devices); }
		}

		public string PresenrationZones
		{
			get { return GKManager.GetCommaSeparatedObjects(new List<ModelBase>(Zones), new List<ModelBase>(GKManager.Zones)); }
		}

		public string PresenrationGuardZones
		{
			get { return GKManager.GetCommaSeparatedObjects(new List<ModelBase>(GuardZones), new List<ModelBase>(GKManager.GuardZones)); }
		}

		public string PresenrationDirections
		{
			get { return GKManager.GetCommaSeparatedObjects(new List<ModelBase>(Directions), new List<ModelBase>(GKManager.Directions)); }
		}

		public string PresenrationMPTs
		{
			get { return GKManager.GetCommaSeparatedObjects(new List<ModelBase>(MPTs), new List<ModelBase>(GKManager.MPTs)); }
		}

		public string PresenrationDelays
		{
			get { return GKManager.GetCommaSeparatedObjects(new List<ModelBase>(Delays), new List<ModelBase>(GKManager.Delays)); }
		}

		public string PresenrationDoors
		{
			get
			{
				var name = GKManager.GetCommaSeparatedObjects(new List<ModelBase>(Doors), new List<ModelBase>(GKManager.Doors));
				return name;
			}
		}

		public string PresenrationPumpStations
		{
			get
			{
				var name = GKManager.GetCommaSeparatedObjects(new List<ModelBase>(PumpStations), new List<ModelBase>(GKManager.PumpStations));
				return name;
			}
		}

		public bool CanSelectDevices
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllDevices || SelectedClauseOperationType == ClauseOperationType.AnyDevice); }
		}

		public bool CanSelectZones
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllZones || SelectedClauseOperationType == ClauseOperationType.AnyZone); }
		}

		public bool CanSelectGuardZones
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllGuardZones || SelectedClauseOperationType == ClauseOperationType.AnyGuardZone); }
		}

		public bool CanSelectDirections
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllDirections || SelectedClauseOperationType == ClauseOperationType.AnyDirection); }
		}

		public bool CanSelectMPTs
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllMPTs || SelectedClauseOperationType == ClauseOperationType.AnyMPT); }
		}

		public bool CanSelectDelays
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllDelays || SelectedClauseOperationType == ClauseOperationType.AnyDelay); }
		}

		public bool CanSelectDoors
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllDoors || SelectedClauseOperationType == ClauseOperationType.AnyDoor); }
		}

		public bool CanSelectPumpStations
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllPumpStations || SelectedClauseOperationType == ClauseOperationType.AnyPumpStation); }
		}

		public RelayCommand SelectDevicesCommand { get; private set; }
		void OnSelectDevices()
		{
			var sourceDevices = new List<GKDevice>();
			foreach (var device in GKManager.Devices)
			{
				if (device.Driver.AvailableStateBits.Contains(SelectedStateType.StateBit))
					sourceDevices.Add(device);
			}
			var devicesSelectationViewModel = new DevicesSelectationViewModel(Devices, sourceDevices);
			if (ServiceFactory.DialogService.ShowModalWindow(devicesSelectationViewModel))
			{
				Devices = devicesSelectationViewModel.DevicesList;
				OnPropertyChanged(() => PresenrationDevices);
			}
		}

		public RelayCommand SelectZonesCommand { get; private set; }
		void OnSelectZones()
		{
			var zonesSelectationViewModel = new ZonesSelectationViewModel(Zones);
			if (DialogService.ShowModalWindow(zonesSelectationViewModel))
			{
				Zones = zonesSelectationViewModel.Zones;
				OnPropertyChanged(() => PresenrationZones);
			}
		}

		public RelayCommand SelectGuardZonesCommand { get; private set; }
		void OnSelectGuardZones()
		{
			var guardZonesSelectationViewModel = new GuardZonesSelectationViewModel(GuardZones);
			if (DialogService.ShowModalWindow(guardZonesSelectationViewModel))
			{
				GuardZones = guardZonesSelectationViewModel.GuardZones;
				OnPropertyChanged(() => PresenrationGuardZones);
			}
		}

		public RelayCommand SelectDirectionCommand { get; private set; }
		void OnSelectDirections()
		{
			var directionsSelectationViewModel = new DirectionsSelectationViewModel(Directions);
			if (DialogService.ShowModalWindow(directionsSelectationViewModel))
			{
				Directions = directionsSelectationViewModel.Directions;
				OnPropertyChanged(() => PresenrationDirections);
			}
		}

		public RelayCommand SelectMPTsCommand { get; private set; }
		void OnSelectMPTs()
		{
			var mptsSelectationViewModel = new MPTsSelectationViewModel(MPTs);
			if (DialogService.ShowModalWindow(mptsSelectationViewModel))
			{
				MPTs = mptsSelectationViewModel.MPTs;
				OnPropertyChanged(() => PresenrationMPTs);
			}
		}

		public RelayCommand SelectDelaysCommand { get; private set; }
		void OnSelectDelays()
		{
			var delaysSelectationViewModel = new DelaysSelectationViewModel(Delays);
			if (DialogService.ShowModalWindow(delaysSelectationViewModel))
			{
				Delays = delaysSelectationViewModel.Delays;
				OnPropertyChanged(() => PresenrationDelays);
			}
		}

		public RelayCommand SelectDoorsCommand { get; private set; }
		void OnSelectDoors()
		{
			var doorsSelectationViewModel = new DoorsSelectationViewModel(Doors);
			if (DialogService.ShowModalWindow(doorsSelectationViewModel))
			{
				Doors = doorsSelectationViewModel.Doors;
				OnPropertyChanged(() => PresenrationDoors);
			}
		}

		public RelayCommand SelectPumpStationsCommand { get; private set; }
		void OnSelectPumpStations()
		{
			var pumpStationsSelectationViewModel = new PumpStationsSelectationViewModel(PumpStations);
			if (DialogService.ShowModalWindow(pumpStationsSelectationViewModel))
			{
				PumpStations = pumpStationsSelectationViewModel.PumpStations;
				OnPropertyChanged(() => PresenrationPumpStations);
			}
		}
	}
}