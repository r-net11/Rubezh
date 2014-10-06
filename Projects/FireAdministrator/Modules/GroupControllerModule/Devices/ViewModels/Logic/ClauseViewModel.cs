using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

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
		GKDevice Device;

		public ClauseViewModel(GKDevice device, GKClause clause)
		{
			Device = device;
			SelectDevicesCommand = new RelayCommand(OnSelectDevices);
			SelectZonesCommand = new RelayCommand(OnSelectZones);
			SelectGuardZonesCommand = new RelayCommand(OnSelectGuardZones);
			SelectDirectionCommand = new RelayCommand(OnSelectDirections);
			SelectMPTsCommand = new RelayCommand(OnSelectMPTs);
			SelectDelaysCommand = new RelayCommand(OnSelectDelays);

			ClauseConditionTypes = Enum.GetValues(typeof(ClauseConditionType)).Cast<ClauseConditionType>().ToList();
			ClauseOperationTypes = Enum.GetValues(typeof(ClauseOperationType)).Cast<ClauseOperationType>().ToList();
			SelectedClauseOperationType = clause.ClauseOperationType;

			Devices = clause.Devices.ToList();
			Zones = clause.Zones.ToList();
			GuardZones = clause.GuardZones.ToList();
			Directions = clause.Directions.ToList();
			MPTs = clause.MPTs.ToList();
			Delays = clause.Delays.ToList();

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
						StateTypes = new ObservableCollection<StateTypeViewModel>();
						StateTypes.Add(new StateTypeViewModel(value, GKStateBit.Norm));
						StateTypes.Add(new StateTypeViewModel(value, GKStateBit.Fire2));
						if (Device.DriverType != GKDriverType.MPT)
						{
							StateTypes.Add(new StateTypeViewModel(value, GKStateBit.Fire1));
							StateTypes.Add(new StateTypeViewModel(value, GKStateBit.On));
							StateTypes.Add(new StateTypeViewModel(value, GKStateBit.Failure));
						}
						break;

					case ClauseOperationType.AllZones:
					case ClauseOperationType.AnyZone:
						StateTypes = new ObservableCollection<StateTypeViewModel>();
						StateTypes.Add(new StateTypeViewModel(value, GKStateBit.Fire2));
						if (Device.DriverType != GKDriverType.MPT)
						{
							StateTypes.Add(new StateTypeViewModel(value, GKStateBit.Fire1));
							StateTypes.Add(new StateTypeViewModel(value, GKStateBit.Attention));
						}
						break;

					case ClauseOperationType.AllGuardZones:
					case ClauseOperationType.AnyGuardZone:
						StateTypes = new ObservableCollection<StateTypeViewModel>();
						StateTypes.Add(new StateTypeViewModel(value, GKStateBit.On));
						StateTypes.Add(new StateTypeViewModel(value, GKStateBit.Off));
						StateTypes.Add(new StateTypeViewModel(value, GKStateBit.Attention));
						break;

					case ClauseOperationType.AllDirections:
					case ClauseOperationType.AnyDirection:
						StateTypes = new ObservableCollection<StateTypeViewModel>()
						{
							new StateTypeViewModel(value, GKStateBit.On)
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
				}
				if (StateTypes.Any(x=>x.StateBit == oldSelectedStateType))
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
				OnPropertyChanged(() => CanSelectDevices);
				OnPropertyChanged(() => CanSelectZones);
				OnPropertyChanged(() => CanSelectGuardZones);
				OnPropertyChanged(() => CanSelectDirections);
				OnPropertyChanged(() => CanSelectMPTs);
				OnPropertyChanged(() => CanSelectDelays);
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
				OnPropertyChanged(() => SelectedStateType);
			}
		}

		public string PresenrationDevices
		{
			get { return GKManager.GetCommaSeparatedDevices(Devices); }
		}

		public string PresenrationZones
		{
			get { return GKManager.GetCommaSeparatedObjects(new List<ModelBase>(Zones)); }
		}

		public string PresenrationGuardZones
		{
			get { return GKManager.GetCommaSeparatedObjects(new List<ModelBase>(GuardZones)); }
		}

		public string PresenrationDirections
		{
			get { return GKManager.GetCommaSeparatedObjects(new List<ModelBase>(Directions)); }
		}

		public string PresenrationMPTs
		{
			get { return GKManager.GetCommaSeparatedObjects(new List<ModelBase>(MPTs)); }
		}

		public string PresenrationDelays
		{
			get { return GKManager.GetCommaSeparatedObjects(new List<ModelBase>(Delays)); }
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

		public RelayCommand SelectDevicesCommand { get; private set; }
		void OnSelectDevices()
		{
			var sourceDevices = new List<GKDevice>();
			foreach (var device in GKManager.Devices)
			{
				if (device.IsNotUsed)
					continue;
				if (Device.DriverType != GKDriverType.GKRele)
				{
					if (!device.Driver.IsDeviceOnShleif && Device.Driver.IsDeviceOnShleif)
						continue;
				}
				if (device.UID == Device.UID)
					continue;
				if (device.Driver.AvailableStateBits.Contains(SelectedStateType.StateBit))
					sourceDevices.Add(device);
			}
			var devicesSelectationViewModel = new DevicesSelectationViewModel(Devices, sourceDevices);
			if (DialogService.ShowModalWindow(devicesSelectationViewModel))
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
				GuardZones = guardZonesSelectationViewModel.Zones;
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
				OnPropertyChanged("PresenrationMPTs");
			}
		}

		public RelayCommand SelectDelaysCommand { get; private set; }
		void OnSelectDelays()
		{
			var delaysSelectationViewModel = new DelaysSelectationViewModel(Delays);
			if (DialogService.ShowModalWindow(delaysSelectationViewModel))
			{
				Delays = delaysSelectationViewModel.Delays;
				OnPropertyChanged("PresenrationDelays");
			}
		}
	}
}