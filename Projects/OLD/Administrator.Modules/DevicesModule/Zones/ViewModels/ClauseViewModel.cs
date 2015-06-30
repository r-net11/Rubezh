using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class ClauseViewModel : BaseViewModel
	{
		public List<Guid> Zones { get; set; }
		Device Device;
		ZoneLogicViewModel _zoneLogicViewModel;

		public ClauseViewModel(ZoneLogicViewModel zoneLogicViewModel, Device device, Clause clause)
		{
			ShowZonesCommand = new RelayCommand(OnShowZones);
			SelectDeviceCommand = new RelayCommand(OnSelectDevice);

			_zoneLogicViewModel = zoneLogicViewModel;
			Device = device;
			Zones = new List<Guid>(
				from zoneUID in clause.ZoneUIDs
				select zoneUID);
			Zones = clause.ZoneUIDs.ToList();
			_selectedState = clause.State;
			SelectedOperation = clause.Operation;

			if (clause.DeviceUIDs == null)
			{
				clause.DeviceUIDs = new List<Guid>();
			}
			SelectedDevices = new List<Device>();
			foreach (var deviceUID in clause.DeviceUIDs)
			{
				var deviceInClause = FiresecManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
				if (deviceInClause != null)
				{
					SelectedDevices.Add(deviceInClause);
				}
			}

			SelectedMROMessageNo = clause.ZoneLogicMROMessageNo;
			SelectedMROMessageType = clause.ZoneLogicMROMessageType;
		}

		public List<ZoneLogicState> States
		{
			get
			{
				switch (Device.Driver.DriverType)
				{
					case DriverType.Valve:
					case DriverType.MDU:
					case DriverType.MRO:
					case DriverType.ASPT:
						return new List<ZoneLogicState>() {
							ZoneLogicState.Fire,
							ZoneLogicState.Attention,
							ZoneLogicState.MPTAutomaticOn,
							ZoneLogicState.MPTOn};

					case DriverType.PumpStation:
					case DriverType.ControlCabinet:
					case DriverType.FanCabinet:
						return new List<ZoneLogicState>() {
							ZoneLogicState.Fire,
							ZoneLogicState.Attention,
							ZoneLogicState.MPTAutomaticOn,
							ZoneLogicState.MPTOn,
							ZoneLogicState.AM1TOn};

					case DriverType.RM_1:
						return new List<ZoneLogicState>() {
									ZoneLogicState.Fire,
									ZoneLogicState.Attention,
									ZoneLogicState.MPTAutomaticOn,
									ZoneLogicState.MPTOn,
									ZoneLogicState.Firefighting,
									ZoneLogicState.Alarm,
									ZoneLogicState.GuardSet,
									ZoneLogicState.GuardUnSet,
									ZoneLogicState.Lamp,
									ZoneLogicState.AM1TOn,
									ZoneLogicState.ShuzOn};

					case DriverType.SonarDirection:
						return new List<ZoneLogicState>() {
									ZoneLogicState.Fire,
									ZoneLogicState.Attention,
									ZoneLogicState.MPTAutomaticOn,
									ZoneLogicState.MPTOn};

					case DriverType.Exit:
						var states = new List<ZoneLogicState>();
						switch (Device.Parent.Driver.DriverType)
						{
							case DriverType.Rubezh_4A:
							case DriverType.USB_Rubezh_4A:
							case DriverType.Rubezh_P:
							case DriverType.USB_Rubezh_P:
								states = new List<ZoneLogicState>() {
									ZoneLogicState.Fire,
									ZoneLogicState.Attention,
									ZoneLogicState.MPTAutomaticOn,
									ZoneLogicState.MPTOn,
									ZoneLogicState.Failure,
									ZoneLogicState.DoubleFire};
								break;

							case DriverType.Rubezh_2OP:
							case DriverType.USB_Rubezh_2OP:
								states.Add(ZoneLogicState.Fire);
								states.Add(ZoneLogicState.Attention);
								states.Add(ZoneLogicState.MPTAutomaticOn);
								states.Add(ZoneLogicState.MPTOn);
								states.Add(ZoneLogicState.Alarm);
								states.Add(ZoneLogicState.GuardSet);
								states.Add(ZoneLogicState.GuardUnSet);
								states.Add(ZoneLogicState.PCN);
								states.Add(ZoneLogicState.Lamp);
								states.Add(ZoneLogicState.Failure);
								states.Add(ZoneLogicState.DoubleFire);
								break;
						}
						if ((Device.IntAddress == 3) || (Device.IntAddress == 4))
							states.Remove(ZoneLogicState.Failure);

						return states;
				}
				return GetAllZoneLogicStates();
			}
		}

		List<ZoneLogicState> GetAllZoneLogicStates()
		{
			var states = new List<ZoneLogicState>();
			states.Add(ZoneLogicState.Fire);
			states.Add(ZoneLogicState.Attention);
			states.Add(ZoneLogicState.MPTAutomaticOn);
			states.Add(ZoneLogicState.MPTOn);
			states.Add(ZoneLogicState.Alarm);
			states.Add(ZoneLogicState.GuardSet);
			states.Add(ZoneLogicState.GuardUnSet);
			//states.Add(ZoneLogicState.PCN);
			states.Add(ZoneLogicState.Lamp);
			states.Add(ZoneLogicState.Failure);
			states.Add(ZoneLogicState.AM1TOn);
			states.Add(ZoneLogicState.ShuzOn);
			states.Add(ZoneLogicState.Firefighting);
			states.Add(ZoneLogicState.PumpStationOn);
			states.Add(ZoneLogicState.PumpStationAutomaticOff);
			return states;
		}

		public List<ZoneLogicOperation> Operations
		{
			get
			{
				var operations = new List<ZoneLogicOperation>();
				operations.Add(ZoneLogicOperation.All);
				operations.Add(ZoneLogicOperation.Any);
				return operations;
			}
		}

		public bool CanSelectOperation
		{
			get
			{
				return (SelectedState == ZoneLogicState.Fire) ||
					(SelectedState == ZoneLogicState.Attention) ||
					(SelectedState == ZoneLogicState.MPTAutomaticOn) ||
					(SelectedState == ZoneLogicState.MPTOn) ||
					(SelectedState == ZoneLogicState.Alarm) ||
					(SelectedState == ZoneLogicState.GuardSet) ||
					(SelectedState == ZoneLogicState.GuardUnSet) ||
					(SelectedState == ZoneLogicState.Firefighting);
			}
		}

		public bool CanSelectZones
		{
			get
			{
				return (SelectedState != ZoneLogicState.Failure && SelectedState != ZoneLogicState.DoubleFire && !CanSelectDevice);
			}
		}

		public bool CanSelectDevice
		{
			get
			{
				return (SelectedState == ZoneLogicState.AM1TOn || SelectedState == ZoneLogicState.ShuzOn);
			}
		}

		void Update()
		{
			Zones = new List<Guid>();
			SelectedDevices = new List<Device>(); ;
			OnPropertyChanged(() => CanSelectOperation);
			OnPropertyChanged(() => CanSelectZones);
			OnPropertyChanged(() => CanSelectDevice);
			OnPropertyChanged(() => PresenrationZones);
			OnPropertyChanged(() => PresenrationSelectedDevice);

			_zoneLogicViewModel.OnCurrentClauseStateChanged(SelectedState);
		}

		public string PresenrationZones
		{
			get { return FiresecManager.FiresecConfiguration.GetCommaSeparatedZones(Zones); }
		}

		ZoneLogicState _selectedState;
		public ZoneLogicState SelectedState
		{
			get { return _selectedState; }
			set
			{
				_selectedState = value;
				OnPropertyChanged(() => SelectedState);
				if (value == ZoneLogicState.PCN)
				{
					SelectedOperation = ZoneLogicOperation.Any;
				}
				Update();
			}
		}

		ZoneLogicOperation? _selectedOperation;
		public ZoneLogicOperation? SelectedOperation
		{
			get { return _selectedOperation; }
			set
			{
				_selectedOperation = value;
				OnPropertyChanged(() => SelectedOperation);
			}
		}

		#region IsMRO_2M
		public bool IsSonar
		{
			get { return Device.Driver.DriverType == DriverType.SonarDirection || Device.Driver.DriverType == DriverType.MRO_2; }
		}

		public List<ZoneLogicMROMessageNo> AvailableMROMessageNos
		{
			get { return Enum.GetValues(typeof(ZoneLogicMROMessageNo)).Cast<ZoneLogicMROMessageNo>().ToList(); }
		}

		ZoneLogicMROMessageNo _selectedMROMessageNo;
		public ZoneLogicMROMessageNo SelectedMROMessageNo
		{
			get { return _selectedMROMessageNo; }
			set
			{
				_selectedMROMessageNo = value;
				OnPropertyChanged(() => SelectedMROMessageNo);
			}
		}

		public List<ZoneLogicMROMessageType> AvailableMROMessageTypes
		{
			get { return Enum.GetValues(typeof(ZoneLogicMROMessageType)).Cast<ZoneLogicMROMessageType>().ToList(); }
		}

		ZoneLogicMROMessageType _selectedMROMessageType;
		public ZoneLogicMROMessageType SelectedMROMessageType
		{
			get { return _selectedMROMessageType; }
			set
			{
				_selectedMROMessageType = value;
				OnPropertyChanged(() => SelectedMROMessageType);
			}
		}
		#endregion

		List<Device> _selectedDevices;
		public List<Device> SelectedDevices
		{
			get { return _selectedDevices; }
			set
			{
				_selectedDevices = value;
				OnPropertyChanged(() => SelectedDevices);
			}
		}

		public string PresenrationSelectedDevice
		{
			get
			{
				var result = "";
				foreach (var device in SelectedDevices)
				{
					result += device.DottedPresentationNameAndAddress + ", ";
				}
				if (result.EndsWith(", "))
					result = result.Remove(result.Length - 2, 2);
				return result;
			}
		}

		bool _showJoinOperator;
		public bool ShowJoinOperator
		{
			get { return _showJoinOperator; }
			set
			{
				_showJoinOperator = value;
				OnPropertyChanged(() => ShowJoinOperator);
			}
		}

		public RelayCommand ShowZonesCommand { get; private set; }
		void OnShowZones()
		{
			var zonesSelectionViewModel = new ZonesSelectionViewModel(Device, Zones, SelectedState);
			if (DialogService.ShowModalWindow(zonesSelectionViewModel))
			{
				var zones = new List<Zone>();
				if (zonesSelectionViewModel.Zones != null)
				{
					foreach (var zoneUID in zonesSelectionViewModel.Zones)
					{
						var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
						if (zone != null)
							zones.Add(zone);
					}
				}
				var zoneUIDs = from Zone zone in zones orderby zone.No select zone.UID;
				Zones = zoneUIDs.ToList();
				OnPropertyChanged(() => PresenrationZones);
			}
		}

		public RelayCommand SelectDeviceCommand { get; private set; }
		void OnSelectDevice()
		{
			var zoneLogicDevicesSelectionViewModel = new ZoneLogicDevicesSelectionViewModel(Device, SelectedDevices, SelectedState);
			if (DialogService.ShowModalWindow(zoneLogicDevicesSelectionViewModel))
			{
				SelectedDevices = zoneLogicDevicesSelectionViewModel.SelectedDevices;
				OnPropertyChanged(() => PresenrationSelectedDevice);
			}
		}
	}
}