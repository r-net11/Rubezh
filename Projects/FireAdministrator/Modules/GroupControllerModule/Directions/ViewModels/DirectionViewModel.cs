using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Helper;
using XFiresecAPI;

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
			Update();
		}

		void InitializeDependences()
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
			OnPropertyChanged("Direction");
			if (Direction.PlanElementUIDs == null)
				Direction.PlanElementUIDs = new List<Guid>();
			_visualizetionState = Direction.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Direction.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => VisualizationState);
		}

		public ObservableCollection<DirectionZoneViewModel> Zones { get; private set; }
		public ObservableCollection<DirectionDeviceViewModel> Devices { get; private set; }
		public ObservableCollection<DeviceViewModel> OutputDevices { get; private set; }

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
		public void ChangeDevices()
		{
			var sourceDevices = new List<XDevice>();
			foreach (var device in XManager.DeviceConfiguration.Devices)
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
		public void ChangeOutputDevices()
		{
			var sourceDevices = new List<XDevice>();
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.IsDeviceOnShleif && device.Driver.HasLogic)
					sourceDevices.Add(device);
			}

			var devicesSelectationViewModel = new DevicesSelectationViewModel(Direction.OutputDevices, sourceDevices);
			if (DialogService.ShowModalWindow(devicesSelectationViewModel))
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

				var devices = devicesSelectationViewModel.DevicesList;
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
		public void Update(XDirection xdirection)
		{
			Direction = xdirection;
			OnPropertyChanged("Direction");
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
			Update();
		}
	}
}