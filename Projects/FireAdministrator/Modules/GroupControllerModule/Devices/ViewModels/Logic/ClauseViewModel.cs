using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ClauseViewModel : BaseViewModel
	{
		public List<XZone> Zones { get; set; }
		public List<XDevice> Devices { get; set; }
		public List<XDirection> Directions { get; set; }
		XDevice Device;

		public ClauseViewModel(XClause clause, XDevice device)
		{
			Device = device;
			SelectZonesCommand = new RelayCommand(OnSelectZones);
			SelectDevicesCommand = new RelayCommand(OnSelectDevices);
			SelectDirectionCommand = new RelayCommand(OnSelectDirections);

			ClauseConditionTypes = Enum.GetValues(typeof(ClauseConditionType)).Cast<ClauseConditionType>().ToList();
			ClauseOperationTypes = Enum.GetValues(typeof(ClauseOperationType)).Cast<ClauseOperationType>().ToList();

			SelectedClauseOperationType = clause.ClauseOperationType;
			Zones = clause.Zones.ToList();
			Devices = clause.Devices.ToList();
			Directions = clause.Directions.ToList();

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
				OnPropertyChanged("SelectedClauseConditionType");
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
						Zones = new List<XZone>();
						Directions = new List<XDirection>();
						StateTypes = new ObservableCollection<XStateBit>();
						StateTypes.Add(XStateBit.Norm);
						StateTypes.Add(XStateBit.Fire2);
						if (Device.Driver.DriverType != XDriverType.MPT)
						{
							StateTypes.Add(XStateBit.Fire1);
							StateTypes.Add(XStateBit.On);
							StateTypes.Add(XStateBit.Failure);
						}
						break;

					case ClauseOperationType.AllZones:
					case ClauseOperationType.AnyZone:
						Devices = new List<XDevice>();
						Directions = new List<XDirection>();
						StateTypes = new ObservableCollection<XStateBit>();
						StateTypes.Add(XStateBit.Fire2);
						if (Device.Driver.DriverType != XDriverType.MPT)
						{
							StateTypes.Add(XStateBit.Fire1);
							StateTypes.Add(XStateBit.Attention);
						}
						break;

					case ClauseOperationType.AllDirections:
					case ClauseOperationType.AnyDirection:
						Zones = new List<XZone>();
						Devices = new List<XDevice>();
						StateTypes = new ObservableCollection<XStateBit>();
						StateTypes.Add(XStateBit.On);
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
				OnPropertyChanged("SelectedClauseOperationType");
				OnPropertyChanged("PresenrationZones");
				OnPropertyChanged("PresenrationDevices");
				OnPropertyChanged("PresenrationDirections");
				OnPropertyChanged("CanSelectZones");
				OnPropertyChanged("CanSelectDevices");
				OnPropertyChanged("CanSelectDirections");
			}
		}

		ObservableCollection<XStateBit> _stateTypes;
		public ObservableCollection<XStateBit> StateTypes
		{
			get { return _stateTypes; }
			set
			{
				_stateTypes = value;
				OnPropertyChanged("StateTypes");
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
				OnPropertyChanged("SelectedStateType");
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

		public string PresenrationZones
		{
			get { return XManager.GetCommaSeparatedZones(Zones); }
		}

		public string PresenrationDevices
		{
			get { return XManager.GetCommaSeparatedDevices(Devices); }
		}

		public string PresenrationDirections
		{
			get { return XManager.GetCommaSeparatedDirections(Directions); }
		}

		public bool CanSelectZones
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllZones || SelectedClauseOperationType == ClauseOperationType.AnyZone); }
		}

		public bool CanSelectDevices
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllDevices || SelectedClauseOperationType == ClauseOperationType.AnyDevice); }
		}

		public bool CanSelectDirections
		{
			get { return (SelectedClauseOperationType == ClauseOperationType.AllDirections || SelectedClauseOperationType == ClauseOperationType.AnyDirection); }
		}

		public RelayCommand SelectZonesCommand { get; private set; }
		void OnSelectZones()
		{
			var zonesSelectationViewModel = new ZonesSelectationViewModel(Zones);
			if (DialogService.ShowModalWindow(zonesSelectationViewModel))
			{
				Zones = zonesSelectationViewModel.Zones;
				OnPropertyChanged("PresenrationZones");
			}
		}

		public RelayCommand SelectDevicesCommand { get; private set; }
		void OnSelectDevices()
		{
			var sourceDevices = new List<XDevice>();
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.IsNotUsed)
					continue;
				if (Device.Driver.DriverType == XDriverType.GKLine || Device.Driver.DriverType == XDriverType.GKRele)
				{

				}
				else
				{
					if (!device.Driver.IsDeviceOnShleif && Device.Driver.IsDeviceOnShleif)
						continue;
				}
				if (device.UID == Device.UID)
					continue;
				if (device.Driver.AvailableStateBits.Contains(SelectedStateType))
					sourceDevices.Add(device);
			}
			var devicesSelectationViewModel = new DevicesSelectationViewModel(Devices, sourceDevices);
			if (DialogService.ShowModalWindow(devicesSelectationViewModel))
			{
				Devices = devicesSelectationViewModel.DevicesList;
				OnPropertyChanged("PresenrationDevices");
			}
		}

		public RelayCommand SelectDirectionCommand { get; private set; }
		void OnSelectDirections()
		{
			var directionsSelectationViewModel = new DirectionsSelectationViewModel(Directions);
			if (DialogService.ShowModalWindow(directionsSelectationViewModel))
			{
				Directions = directionsSelectationViewModel.Directions;
				OnPropertyChanged("PresenrationDirections");
			}
		}
	}
}