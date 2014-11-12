using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DirectionViewModel : BaseViewModel
	{
		private VisualizationState _visualizetionState;
		public GKDirection Direction { get; set; }

		public DirectionViewModel(GKDirection direction)
		{
			Direction = direction;
			Zones = new ObservableCollection<DirectionZoneViewModel>();
			Devices = new ObservableCollection<DirectionDeviceViewModel>();
			OutputDevices = new ObservableCollection<DeviceViewModel>();
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
		}

		public void Update()
		{
			InitializeDependences();
			OnPropertyChanged(() => Direction);
			if (Direction.PlanElementUIDs == null)
				Direction.PlanElementUIDs = new List<Guid>();
			_visualizetionState = Direction.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Direction.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => VisualizationState);
		}

		public ObservableCollection<DirectionZoneViewModel> Zones { get; private set; }
		public ObservableCollection<DirectionDeviceViewModel> Devices { get; private set; }
		public ObservableCollection<DeviceViewModel> OutputDevices { get; private set; }

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
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		DeviceViewModel _selectedOutputDevice;
		public DeviceViewModel SelectedOutputDevice
		{
			get { return _selectedOutputDevice; }
			set
			{
				_selectedOutputDevice = value;
				OnPropertyChanged(() => SelectedOutputDevice);
			}
		}

		public void ChangeZones()
		{
			var zonesSelectationViewModel = new ZonesSelectationViewModel(Direction.InputZones);
			if (DialogService.ShowModalWindow(zonesSelectationViewModel))
			{
				GKManager.ChangeDirectionZones(Direction, zonesSelectationViewModel.Zones);
				InitializeDependences();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public void DeleteZone(GKDirectionZone directionZone)
		{
			List<GKDirectionZone> directionZones = Direction.DirectionZones;
			directionZones.Remove(directionZone);
			var zones = new List<GKZone>();
			directionZones.ForEach(x => zones.Add(x.Zone));
			GKManager.ChangeDirectionZones(Direction, zones);
			InitializeDependences();
			ServiceFactory.SaveService.GKChanged = true;
		}

		public void ChangeDevices()
		{
			var sourceDevices = new List<GKDevice>();

			foreach (var device in GKManager.Devices)
			{
				if (device.Driver.IsDeviceOnShleif)
					sourceDevices.Add(device);
			}

			var devicesSelectationViewModel = new DevicesSelectationViewModel(Direction.InputDevices, sourceDevices);
			if (DialogService.ShowModalWindow(devicesSelectationViewModel))
			{
				GKManager.ChangeDirectionDevices(Direction, devicesSelectationViewModel.DevicesList);
				InitializeDependences();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public void DeleteDevice(GKDevice device)
		{
			List<GKDevice> devices = new List<GKDevice>(Direction.InputDevices);
			devices.Remove(device);
			GKManager.ChangeDirectionDevices(Direction, devices);
			InitializeDependences();
			ServiceFactory.SaveService.GKChanged = true;
		}

		public void ChangeOutputDevices()
		{
			var sourceDevices = new List<GKDevice>();
			foreach (var device in GKManager.Devices)
			{
				if (device.Driver.IsDeviceOnShleif && device.Driver.HasLogic && !device.IsInMPT)
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

		public void SetNewOutputDevices(List<GKDevice> devices)
		{
			foreach (var device in Direction.OutputDevices)
			{
				foreach (var clause in device.Logic.OnClausesGroup.Clauses)
				{
					clause.Directions.RemoveAll(x => x == Direction);
					clause.DirectionUIDs.RemoveAll(x => x == Direction.UID);
				}
				device.Logic.OnClausesGroup.Clauses.RemoveAll(x => x.Zones.Count + x.Devices.Count + x.Directions.Count == 0);
				device.OnChanged();
			}
			Direction.OutputDevices.Clear();

			Direction.OutputDevices = devices;
			foreach (var device in Direction.OutputDevices)
			{
				var alreadyHasDirectionClause = false;
				foreach (var clause in device.Logic.OnClausesGroup.Clauses)
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
					var clause = new GKClause()
					{
						ClauseOperationType = ClauseOperationType.AnyDirection,
						StateType = GKStateBit.On,
						Directions = new List<GKDirection>() { Direction },
						DirectionUIDs = new List<Guid>() { Direction.UID }
					};

					device.Logic.OnClausesGroup.Clauses.Add(clause);
					device.Directions.Add(Direction);
					device.OnChanged();
				}
			}
		}

		public void DeleteOutputDevice(GKDevice deviceToRemove)
		{
			var devices = new List<GKDevice>(Direction.OutputDevices);
			devices.Remove(deviceToRemove);
			foreach (var device in Direction.OutputDevices)
			{
				foreach (var clause in device.Logic.OnClausesGroup.Clauses)
				{
					clause.Directions.RemoveAll(x => x == Direction);
					clause.DirectionUIDs.RemoveAll(x => x == Direction.UID);
				}
				device.Logic.OnClausesGroup.Clauses.RemoveAll(x => x.Zones.Count + x.Devices.Count + x.Directions.Count == 0);
				device.OnChanged();
			}
			Direction.OutputDevices.Clear();
			foreach (var device in devices)
			{
				var alreadyHasDirectionClause = false;
				foreach (var clause in device.Logic.OnClausesGroup.Clauses)
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
					var clause = new GKClause()
					{
						ClauseOperationType = ClauseOperationType.AnyDirection,
						StateType = GKStateBit.On,
						Directions = new List<GKDirection>() { Direction },
						DirectionUIDs = new List<Guid>() { Direction.UID }
					};

					device.Logic.OnClausesGroup.Clauses.Add(clause);
					device.Directions.Add(Direction);
					device.OnChanged();
				}
			}
			Direction.OutputDevices = devices;

			InitializeDependences();
			ServiceFactory.SaveService.GKChanged = true;
		}

		public string Name
		{
			get { return Direction.Name; }
			set
			{
				Direction.Name = value;
				Direction.OnChanged();
				OnPropertyChanged(() => Name);
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
				OnPropertyChanged(() => Description);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		public VisualizationState VisualizationState
		{
			get { return _visualizetionState; }
		}
		public void Update(GKDirection direction)
		{
			Direction = direction;
			OnPropertyChanged(() => Direction);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
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