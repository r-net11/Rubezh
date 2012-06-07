using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class ClauseViewModel : BaseViewModel
	{
		public List<ulong> Zones { get; set; }
		Device _device;
		ZoneLogicViewModel _zoneLogicViewModel;

		public ClauseViewModel(ZoneLogicViewModel zoneLogicViewModel, Device device, Clause clause)
		{
			ShowZonesCommand = new RelayCommand(OnShowZones);
			SelectDeviceCommand = new RelayCommand(OnSelectDevice);

			_zoneLogicViewModel = zoneLogicViewModel;
			_device = device;
			Zones = clause.Zones.ToList();
			_selectedState = clause.State;
			SelectedOperation = clause.Operation;

			if (clause.DeviceUID != Guid.Empty)
				SelectedDevice = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.DeviceUID);
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
						return new List<ZoneLogicState>() {
							ZoneLogicState.Fire,
							ZoneLogicState.Attention,
							ZoneLogicState.MPTAutomaticOn,
							ZoneLogicState.MPTOn};

					case DriverType.Exit:
						switch (_device.Parent.Driver.DriverType)
						{
							case DriverType.Rubezh_4A:
							case DriverType.USB_Rubezh_4A:
								return new List<ZoneLogicState>() {
									ZoneLogicState.Fire,
									ZoneLogicState.Attention,
									ZoneLogicState.MPTAutomaticOn,
									ZoneLogicState.MPTOn,
									ZoneLogicState.Failure};

							case DriverType.BUNS_2:
							case DriverType.USB_BUNS_2:
								return new List<ZoneLogicState>() {
									ZoneLogicState.Fire,
									ZoneLogicState.Attention,
									ZoneLogicState.MPTAutomaticOn,
									ZoneLogicState.MPTOn,
									ZoneLogicState.PumpStationOn,
									ZoneLogicState.PumpStationAutomaticOff,
									ZoneLogicState.Failure};

							case DriverType.Rubezh_2OP:
							case DriverType.USB_Rubezh_2OP:
								var states = Enum.GetValues(typeof(ZoneLogicState)).Cast<ZoneLogicState>().ToList();
								states.Remove(ZoneLogicState.PumpStationOn);
								states.Remove(ZoneLogicState.PumpStationAutomaticOff);
								return states;
						}
						break;

					case DriverType.RM_1:
						var rmStates = Enum.GetValues(typeof(ZoneLogicState)).Cast<ZoneLogicState>().ToList();
						rmStates.Remove(ZoneLogicState.PumpStationOn);
						rmStates.Remove(ZoneLogicState.PumpStationAutomaticOff);
						rmStates.Remove(ZoneLogicState.Failure);
						return rmStates;
				}
				return Enum.GetValues(typeof(ZoneLogicState)).Cast<ZoneLogicState>().ToList();
			}
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
					(SelectedState == ZoneLogicState.GuardUnSet);
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
			get
			{
				return (SelectedState == ZoneLogicState.AM1TOn);
			}
		}

		void Update()
		{
			OnPropertyChanged("CanSelectOperation");
			OnPropertyChanged("CanSelectZones");
			OnPropertyChanged("CanSelectDevice");
			Zones = new List<ulong>();
			SelectedDevice = null;

			_zoneLogicViewModel.OnCurrentClauseStateChanged(SelectedState);
		}

		public string PresenrationZones
		{
			get
			{
				if (Zones.Count > 0)
				{
					var orderedZones = Zones.OrderBy(x => x).ToList();
					ulong prevZoneNo = orderedZones[0];
					List<List<ulong>> groupOfZones = new List<List<ulong>>();

					for (int i = 0; i < orderedZones.Count; i++)
					{
						var zoneNo = orderedZones[i];
						var haveZonesBetween = FiresecManager.DeviceConfiguration.Zones.Any(x => (x.No > prevZoneNo) && (x.No < zoneNo));
						if (haveZonesBetween)
						{
							groupOfZones.Add(new List<ulong>() { zoneNo });
						}
						else
						{
							if (groupOfZones.Count == 0)
							{
								groupOfZones.Add(new List<ulong>() { zoneNo });
							}
							else
							{
								groupOfZones.Last().Add(zoneNo);
							}
						}
						prevZoneNo = zoneNo;
					}

					var presenrationZones = new StringBuilder();
					for (int i = 0; i < groupOfZones.Count; i++)
					{
						var zoneGroup = groupOfZones[i];

						if (i > 0)
							presenrationZones.Append(", ");

						presenrationZones.Append(zoneGroup.First().ToString());
						if (zoneGroup.Count > 1)
						{
							presenrationZones.Append(" - " + zoneGroup.Last().ToString());
						}
					}

					return presenrationZones.ToString();
				}
				return "";
			}
		}

		ZoneLogicState _selectedState;
		public ZoneLogicState SelectedState
		{
			get { return _selectedState; }
			set
			{
				_selectedState = value;
				OnPropertyChanged("SelectedState");
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
				Zones = zonesSelectionViewModel.Zones;
				OnPropertyChanged("PresenrationZones");
			}
		}

		public RelayCommand SelectDeviceCommand { get; private set; }
		void OnSelectDevice()
		{
			var zoneLogicDeviceSelectionViewModel = new ZoneLogicDeviceSelectionViewModel(_device.Parent);
			if (DialogService.ShowModalWindow(zoneLogicDeviceSelectionViewModel))
				SelectedDevice = zoneLogicDeviceSelectionViewModel.SelectedDevice;
		}
	}
}