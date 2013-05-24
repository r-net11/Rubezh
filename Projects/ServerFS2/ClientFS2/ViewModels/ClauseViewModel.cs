using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace ClientFS2.ViewModels
{
	public class ClauseViewModel : BaseViewModel
	{
		public List<Guid> Zones { get; set; }
		Device _device;
		ZoneLogicViewModel _zoneLogicViewModel;

		public ClauseViewModel(ZoneLogicViewModel zoneLogicViewModel, Device device, Clause clause)
		{
			ShowZonesCommand = new RelayCommand(OnShowZones);
			SelectDeviceCommand = new RelayCommand(OnSelectDevice);

			_zoneLogicViewModel = zoneLogicViewModel;
			_device = device;
			Zones = new List<Guid>(
				from zoneUID in clause.ZoneUIDs
				select zoneUID);
			Zones = clause.ZoneUIDs.ToList();
			_selectedState = clause.State;
			SelectedOperation = clause.Operation;

			if (clause.DeviceUID != Guid.Empty)
				SelectedDevice = FiresecManager.Devices.FirstOrDefault(x => x.UID == clause.DeviceUID);

			SelectedMROMessageNo = clause.ZoneLogicMROMessageNo;
			SelectedMROMessageType = clause.ZoneLogicMROMessageType;
		}

		public List<ZoneLogicState> States
		{
			get
			{
				switch (_device.Driver.DriverType)
				{
					case DriverType.Valve:
					case DriverType.PumpStation:
					case DriverType.MDU:
					case DriverType.MRO:
					case DriverType.ASPT:
					case DriverType.Fan:
						return new List<ZoneLogicState>() {
							ZoneLogicState.Fire,
							ZoneLogicState.Attention,
							ZoneLogicState.MPTAutomaticOn,
							ZoneLogicState.MPTOn};

					case DriverType.Exit:
						var states = new List<ZoneLogicState>();
						switch (_device.Parent.Driver.DriverType)
						{
							case DriverType.Rubezh_4A:
							case DriverType.USB_Rubezh_4A:
								states = new List<ZoneLogicState>() {
									ZoneLogicState.Fire,
									ZoneLogicState.Attention,
									ZoneLogicState.MPTAutomaticOn,
									ZoneLogicState.MPTOn,
									ZoneLogicState.Failure};
								break;

							case DriverType.BUNS_2:
							case DriverType.USB_BUNS_2:
								states = new List<ZoneLogicState>() {
									ZoneLogicState.Fire,
									ZoneLogicState.Attention,
									ZoneLogicState.MPTAutomaticOn,
									ZoneLogicState.MPTOn,
									ZoneLogicState.PumpStationOn,
									ZoneLogicState.PumpStationAutomaticOff,
									ZoneLogicState.Failure};
								break;

							case DriverType.Rubezh_2OP:
							case DriverType.USB_Rubezh_2OP:
								states = GetAllZoneLogicStates();
								states.Remove(ZoneLogicState.PumpStationOn);
								states.Remove(ZoneLogicState.PumpStationAutomaticOff);
								states.Remove(ZoneLogicState.AM1TOn);
								states.Remove(ZoneLogicState.Firefighting);
								states.Add(ZoneLogicState.PCN);
								break;
						}
						if ((_device.IntAddress == 3) || (_device.IntAddress == 4))
							states.Remove(ZoneLogicState.Failure);
						return states;

					case DriverType.RM_1:
						var rmStates = GetAllZoneLogicStates();
						rmStates.Remove(ZoneLogicState.PumpStationOn);
						rmStates.Remove(ZoneLogicState.PumpStationAutomaticOff);
						rmStates.Remove(ZoneLogicState.Failure);
						return rmStates;
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
				return (SelectedState != ZoneLogicState.Failure) &&
					(SelectedState != ZoneLogicState.AM1TOn);
			}
		}

		public bool CanSelectDevice
		{
			get { return (SelectedState == ZoneLogicState.AM1TOn); }
		}

		void Update()
		{
			Zones = new List<Guid>();
			SelectedDevice = null;
			OnPropertyChanged("CanSelectOperation");
			OnPropertyChanged("CanSelectZones");
			OnPropertyChanged("CanSelectDevice");
			OnPropertyChanged("PresenrationZones");

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
				OnPropertyChanged("SelectedState");
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
				OnPropertyChanged("SelectedOperation");
			}
		}

		#region IsMRO_2M
		public bool IsSonar
		{
			get { return _device.Driver.DriverType == DriverType.SonarDirection || _device.Driver.DriverType == DriverType.MRO_2; }
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
				OnPropertyChanged("SelectedMROMessageNo");
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
				OnPropertyChanged("SelectedMROMessageType");
			}
		}
		#endregion

		Device _selectedDevice;
		public Device SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");
			}
		}

		bool _showJoinOperator;
		public bool ShowJoinOperator
		{
			get { return _showJoinOperator; }
			set
			{
				_showJoinOperator = value;
				OnPropertyChanged("ShowJoinOperator");
			}
		}

		public RelayCommand ShowZonesCommand { get; private set; }
		void OnShowZones()
		{
			var zonesSelectionViewModel = new ZonesSelectionViewModel(_device, Zones, SelectedState);
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
				//Zones = zonesSelectionViewModel.Zones;
				OnPropertyChanged("PresenrationZones");
			}
		}

		public RelayCommand SelectDeviceCommand { get; private set; }
		void OnSelectDevice()
		{
			var zoneLogicDeviceSelectionViewModel = new ZoneLogicDeviceSelectionViewModel(_device.ParentPanel);
			if (DialogService.ShowModalWindow(zoneLogicDeviceSelectionViewModel))
				SelectedDevice = zoneLogicDeviceSelectionViewModel.SelectedDevice;
		}
	}
}