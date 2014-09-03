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

namespace GKModule.ViewModels
{
	public class ClauseViewModel : BaseViewModel
	{
		public List<XDevice> Devices { get; set; }
		public List<XZone> Zones { get; set; }
		public List<XDirection> Directions { get; set; }
		public List<XMPT> MPTs { get; set; }
		public List<XDelay> Delays { get; set; }
		XDevice Device;

		public ClauseViewModel(XDevice device, XClause clause)
		{
			Device = device;
			SelectDevicesCommand = new RelayCommand(OnSelectDevices);
			SelectZonesCommand = new RelayCommand(OnSelectZones);
			SelectDirectionCommand = new RelayCommand(OnSelectDirections);
			SelectMPTsCommand = new RelayCommand(OnSelectMPTs);
			SelectDelaysCommand = new RelayCommand(OnSelectDelays);

			ClauseConditionTypes = Enum.GetValues(typeof(ClauseConditionType)).Cast<ClauseConditionType>().ToList();
			ClauseOperationTypes = Enum.GetValues(typeof(ClauseOperationType)).Cast<ClauseOperationType>().ToList();

			SelectedClauseOperationType = clause.ClauseOperationType;
			Devices = clause.Devices.ToList();
			Zones = clause.Zones.ToList();
			Directions = clause.Directions.ToList();
			MPTs = clause.MPTs.ToList();
			Delays = clause.Delays.ToList();

			SelectedClauseConditionType = clause.ClauseConditionType;
			SelectedStateType = clause.StateType;
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
				var oldSelectedStateType = SelectedStateType;

				switch (value)
				{
					case ClauseOperationType.AllDevices:
					case ClauseOperationType.AnyDevice:
						StateTypes = new ObservableCollection<XStateBit>();
						StateTypes.Add(XStateBit.Norm);
						StateTypes.Add(XStateBit.Fire2);
						if (Device.DriverType != XDriverType.MPT)
						{
							StateTypes.Add(XStateBit.Fire1);
							StateTypes.Add(XStateBit.On);
							StateTypes.Add(XStateBit.Failure);
						}
						break;

					case ClauseOperationType.AllZones:
					case ClauseOperationType.AnyZone:
						StateTypes = new ObservableCollection<XStateBit>();
						StateTypes.Add(XStateBit.Fire2);
						if (Device.DriverType != XDriverType.MPT)
						{
							StateTypes.Add(XStateBit.Fire1);
							StateTypes.Add(XStateBit.Attention);
						}
						break;

					case ClauseOperationType.AllDirections:
					case ClauseOperationType.AnyDirection:
						StateTypes = new ObservableCollection<XStateBit>()
						{
							XStateBit.On
						};
						break;

					case ClauseOperationType.AllMPTs:
					case ClauseOperationType.AnyMPT:
						StateTypes = new ObservableCollection<XStateBit>()
						{
							XStateBit.On,
							XStateBit.Off,
							XStateBit.TurningOn,
							XStateBit.Norm
						};
						break;

					case ClauseOperationType.AllDelays:
					case ClauseOperationType.AnyDelay:
						StateTypes = new ObservableCollection<XStateBit>()
						{
							XStateBit.On,
							XStateBit.Off,
							XStateBit.TurningOn,
							XStateBit.Norm
						};
						break;
				}
				if (StateTypes.Contains(oldSelectedStateType))
				{
					SelectedStateType = StateTypes.FirstOrDefault(x => x == oldSelectedStateType);
				}
				else
				{
					SelectedStateType = StateTypes.FirstOrDefault();
				}
				OnPropertyChanged(() => SelectedClauseOperationType);
				OnPropertyChanged(() => PresenrationDevices);
				OnPropertyChanged(() => PresenrationZones);
				OnPropertyChanged(() => PresenrationDirections);
				OnPropertyChanged(() => PresenrationMPTs);
				OnPropertyChanged(() => PresenrationDelays);
				OnPropertyChanged(() => CanSelectDevices);
				OnPropertyChanged(() => CanSelectZones);
				OnPropertyChanged(() => CanSelectDirections);
				OnPropertyChanged(() => CanSelectMPTs);
				OnPropertyChanged(() => CanSelectDelays);
			}
		}

		ObservableCollection<XStateBit> _stateTypes;
		public ObservableCollection<XStateBit> StateTypes
		{
			get { return _stateTypes; }
			set
			{
				_stateTypes = value;
				OnPropertyChanged(() => StateTypes);
			}
		}

		XStateBit _selectedStateType;
		public XStateBit SelectedStateType
		{
			get { return _selectedStateType; }
			set
			{
				if (!StateTypes.Contains(value))
					value = StateTypes.FirstOrDefault();
				_selectedStateType = value;
				OnPropertyChanged(() => SelectedStateType);
			}
		}

		public string PresenrationDevices
		{
			get { return XManager.GetCommaSeparatedDevices(Devices); }
		}

		public string PresenrationZones
		{
			get { return XManager.GetCommaSeparatedObjects(new List<INamedBase>(Zones)); }
		}

		public string PresenrationDirections
		{
			get { return XManager.GetCommaSeparatedObjects(new List<INamedBase>(Directions)); }
		}

		public string PresenrationMPTs
		{
			get { return XManager.GetCommaSeparatedObjects(new List<INamedBase>(MPTs)); }
		}

		public string PresenrationDelays
		{
			get { return XManager.GetCommaSeparatedObjects(new List<INamedBase>(Delays)); }
		}

		public bool CanSelectDevices
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllDevices || SelectedClauseOperationType == ClauseOperationType.AnyDevice); }
		}

		public bool CanSelectZones
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllZones || SelectedClauseOperationType == ClauseOperationType.AnyZone); }
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
			var sourceDevices = new List<XDevice>();
			foreach (var device in XManager.Devices)
			{
				if (device.IsNotUsed)
					continue;
				if (Device.DriverType != XDriverType.GKRele)
				{
					if (!device.Driver.IsDeviceOnShleif && Device.Driver.IsDeviceOnShleif)
						continue;
				}
				if (device.BaseUID == Device.BaseUID)
					continue;
				if (device.Driver.AvailableStateBits.Contains(SelectedStateType))
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