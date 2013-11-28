using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Common;

namespace GKModule.ViewModels
{
	public class DirectionViewModel : BaseViewModel
	{
		private VisualizationState _visualizetionState;
		public XDirection Direction { get; set; }

		public DirectionViewModel(XDirection direction)
		{
			Direction = direction;
			Zones = new ObservableCollection<DirectionZoneViewModel>();
			Devices = new ObservableCollection<DirectionDeviceViewModel>();
			OutputDevices = new ObservableCollection<DeviceViewModel>();
			NSDevices = new ObservableCollection<DeviceViewModel>();
			Update();
		}

		public void InitializeDependences()
		{
			Zones.Clear();
			foreach (var directionZone in Direction.DirectionZones)
			{
				var directionZoneViewModel = new DirectionZoneViewModel(directionZone);
				Zones.Add(directionZoneViewModel);
			}

			Devices.Clear();
			foreach (var directionDevice in Direction.DirectionDevices)
			{
				var directionDeviceViewModel = new DirectionDeviceViewModel(directionDevice);
				Devices.Add(directionDeviceViewModel);
			}

			OutputDevices.Clear();
			foreach (var outputDevice in Direction.OutputDevices)
			{
				var deviceViewModel = new DeviceViewModel(outputDevice);
				OutputDevices.Add(deviceViewModel);
			}

			NSDevices.Clear();
			{
				foreach (var nsDeviceUID in Direction.NSDeviceUIDs)
				{
					var device = XManager.Devices.FirstOrDefault(x => x.UID == nsDeviceUID);
					if (device != null)
					{
						switch (device.DriverType)
						{
							case XDriverType.AM1_T:
							case XDriverType.Pump:
							case XDriverType.RSR2_Bush:
								var deviceViewModel = new DeviceViewModel(device);
								NSDevices.Add(deviceViewModel);
								break;
						}
					}
				}
			}
		}

		public void Update()
		{
			if (Direction.IsNS)
			{
				if (!Direction.OutputDevices.All(x => x.DriverType == XDriverType.AM1_T || x.DriverType == XDriverType.Pump || x.DriverType == XDriverType.RSR2_Bush))
					SetNewOutputDevices(new List<XDevice>());
			}
			else
			{
				Direction.NSDeviceUIDs = new List<Guid>();
			}

			InitializeDependences();
			OnPropertyChanged("Direction");
			if (Direction.PlanElementUIDs == null)
				Direction.PlanElementUIDs = new List<Guid>();
			_visualizetionState = Direction.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Direction.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => VisualizationState);
		}

		public ObservableCollection<DirectionZoneViewModel> Zones { get; private set; }
		public ObservableCollection<DirectionDeviceViewModel> Devices { get; private set; }
		public ObservableCollection<DeviceViewModel> OutputDevices { get; private set; }
		public ObservableCollection<DeviceViewModel> NSDevices { get; private set; }

		DirectionZoneViewModel _selectedZone;
		public DirectionZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged("SelectedZone");
			}
		}

		DirectionDeviceViewModel _selectedDevice;
		public DirectionDeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");
			}
		}

		DeviceViewModel _selectedOutputDevice;
		public DeviceViewModel SelectedOutputDevice
		{
			get { return _selectedOutputDevice; }
			set
			{
				_selectedOutputDevice = value;
				OnPropertyChanged("SelectedOutputDevice");
			}
		}

		DeviceViewModel _selectedNSDevice;
		public DeviceViewModel SelectedNSDevice
		{
			get { return _selectedNSDevice; }
			set
			{
				_selectedNSDevice = value;
				OnPropertyChanged("SelectedNSDevice");
			}
		}

		public void ChangeZones()
		{
			var zonesSelectationViewModel = new ZonesSelectationViewModel(Direction.InputZones);
			if (DialogService.ShowModalWindow(zonesSelectationViewModel))
			{
				XManager.ChangeDirectionZones(Direction, zonesSelectationViewModel.Zones);
				InitializeDependences();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public void DeleteZone(XDirectionZone directionZone)
		{
			List<XDirectionZone> directionZones = Direction.DirectionZones;
			directionZones.Remove(directionZone);
			var zones = new List<XZone>();
			directionZones.ForEach(x => zones.Add(x.Zone));
			XManager.ChangeDirectionZones(Direction, zones);
			InitializeDependences();
			ServiceFactory.SaveService.GKChanged = true;
		}

		public void ChangeDevices()
		{
			var sourceDevices = new List<XDevice>();

			foreach (var device in XManager.Devices)
			{
				if (device.Driver.IsDeviceOnShleif)
					sourceDevices.Add(device);
			}

			var devicesSelectationViewModel = new DevicesSelectationViewModel(Direction.InputDevices, sourceDevices);
			if (DialogService.ShowModalWindow(devicesSelectationViewModel))
			{
				XManager.ChangeDirectionDevices(Direction, devicesSelectationViewModel.DevicesList);
				InitializeDependences();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public void DeleteDevice(XDevice device)
		{
			List<XDevice> devices = new List<XDevice>(Direction.InputDevices);
			devices.Remove(device);
			XManager.ChangeDirectionDevices(Direction, devices);
			InitializeDependences();
			ServiceFactory.SaveService.GKChanged = true;
		}

		public void ChangeOutputDevices()
		{
			var sourceDevices = new List<XDevice>();
			foreach (var device in XManager.Devices)
			{
				if (device.Driver.IsDeviceOnShleif && device.Driver.HasLogic)
					sourceDevices.Add(device);
			}

			var devicesSelectationViewModel = new DevicesSelectationViewModel(Direction.OutputDevices, sourceDevices);
			if (DialogService.ShowModalWindow(devicesSelectationViewModel))
			{
				SetNewOutputDevices(devicesSelectationViewModel.DevicesList);
				InitializeDependences();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public void SetNewOutputDevices(List<XDevice> devices)
		{
			foreach (var device in Direction.OutputDevices)
			{
				foreach (var clause in device.DeviceLogic.Clauses)
				{
					clause.Directions.RemoveAll(x => x == Direction);
					clause.DirectionUIDs.RemoveAll(x => x == Direction.UID);
				}
				device.DeviceLogic.Clauses.RemoveAll(x => x.Zones.Count + x.Devices.Count + x.Directions.Count == 0);
				device.OnChanged();
			}
			Direction.OutputDevices.Clear();

			Direction.OutputDevices = devices;
			if (!Direction.IsNS)
			{
				foreach (var device in Direction.OutputDevices)
				{
					var alreadyHasDirectionClause = false;
					foreach (var clause in device.DeviceLogic.Clauses)
					{
						if ((clause.ClauseOperationType == ClauseOperationType.AllDirections) || (clause.ClauseOperationType == ClauseOperationType.AnyDirection))
						{
							alreadyHasDirectionClause = true;
							if (!clause.Directions.Contains(Direction))
							{
								clause.Directions.Add(Direction);
								clause.DirectionUIDs.Add(Direction.UID);
								device.Directions.Add(Direction);
								device.OnChanged();
							}
						}
					}
					if (!alreadyHasDirectionClause)
					{
						var clause = new XClause()
						{
							ClauseOperationType = ClauseOperationType.AnyDirection,
							StateType = XStateBit.On,
							Directions = new List<XDirection>() { Direction },
							DirectionUIDs = new List<Guid>() { Direction.UID }
						};

						device.DeviceLogic.Clauses.Add(clause);
						device.Directions.Add(Direction);
						device.OnChanged();
					}
				}
			}
		}

		public void DeleteOutputDevice(XDevice deviceToRemove)
		{
			var devices = new List<XDevice>(Direction.OutputDevices);
			devices.Remove(deviceToRemove);
			foreach (var device in Direction.OutputDevices)
			{
				foreach (var clause in device.DeviceLogic.Clauses)
				{
					clause.Directions.RemoveAll(x => x == Direction);
					clause.DirectionUIDs.RemoveAll(x => x == Direction.UID);
				}
				device.DeviceLogic.Clauses.RemoveAll(x => x.Zones.Count + x.Devices.Count + x.Directions.Count == 0);
				device.OnChanged();
			}
			Direction.OutputDevices.Clear();
			foreach (var device in devices)
			{
				var alreadyHasDirectionClause = false;
				foreach (var clause in device.DeviceLogic.Clauses)
				{
					if ((clause.ClauseOperationType == ClauseOperationType.AllDirections) || (clause.ClauseOperationType == ClauseOperationType.AnyDirection))
					{
						alreadyHasDirectionClause = true;
						if (!clause.Directions.Contains(Direction))
						{
							clause.Directions.Add(Direction);
							clause.DirectionUIDs.Add(Direction.UID);
							device.Directions.Add(Direction);
							device.OnChanged();
						}
					}
				}
				if (!alreadyHasDirectionClause)
				{
					var clause = new XClause()
					{
						ClauseOperationType = ClauseOperationType.AnyDirection,
						StateType = XStateBit.On,
						Directions = new List<XDirection>() { Direction },
						DirectionUIDs = new List<Guid>() { Direction.UID }
					};

					device.DeviceLogic.Clauses.Add(clause);
					device.Directions.Add(Direction);
					device.OnChanged();
				}
			}
			Direction.OutputDevices = devices;

			InitializeDependences();
			ServiceFactory.SaveService.GKChanged = true;
		}

		public void ChangeNSDevices()
		{
			var sourceDevices = new List<XDevice>();
			foreach (var device in XManager.Devices)
			{
				if ((device.DriverType == XDriverType.Pump && (device.IntAddress <= 8 || device.IntAddress == 12 || device.IntAddress ==  14)) ||
					device.DriverType == XDriverType.RSR2_Bush || device.DriverType == XDriverType.AM1_T)
					sourceDevices.Add(device);
			}

			foreach (var device in Direction.NSDevices)
			{
				device.NSDirections.Remove(Direction);
			}

			var nsDevces = new List<XDevice>();
			foreach (var nsDeviceUID in Direction.NSDeviceUIDs)
			{
				var nsDevice = XManager.Devices.FirstOrDefault(x => x.UID == nsDeviceUID);
				if (nsDevice != null)
				{
					nsDevces.Add(nsDevice);
				}
			}

			var devicesSelectationViewModel = new DevicesSelectationViewModel(nsDevces, sourceDevices);
			if (DialogService.ShowModalWindow(devicesSelectationViewModel))
			{
				Direction.NSDeviceUIDs = new List<Guid>();
				Direction.NSDevices = new List<XDevice>();
				foreach (var device in devicesSelectationViewModel.DevicesList)
				{
					Direction.NSDeviceUIDs.Add(device.UID);
					Direction.NSDevices.Add(device);
					device.NSDirections.Add(Direction);
				}
				InitializeDependences();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string Name
		{
			get { return Direction.Name; }
			set
			{
				Direction.Name = value;
				Direction.OnChanged();
				OnPropertyChanged("Name");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		public string Description
		{
			get { return Direction.Description; }
			set
			{
				Direction.Description = value;
				Direction.OnChanged();
				OnPropertyChanged("Description");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		public VisualizationState VisualizationState
		{
			get { return _visualizetionState; }
		}
		public void Update(XDirection direction)
		{
			Direction = direction;
			OnPropertyChanged("Direction");
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
			Update();
		}

		#region OPC
		public bool IsOPCUsed
		{
			get { return Direction.IsOPCUsed; }
			set
			{
				Direction.IsOPCUsed = value;
				OnPropertyChanged("IsOPCUsed");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		#endregion
	}
}