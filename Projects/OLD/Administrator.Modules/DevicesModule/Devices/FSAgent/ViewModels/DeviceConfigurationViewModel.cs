using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class DeviceConfigurationViewModel : DialogViewModel
	{
		DeviceConfiguration RemoteDeviceConfiguration;
		Device LocalRootDevice;
		Device RemoteRootDevice;
		Device LocalRootClone;
		Device RemoteRootClone;

		public DeviceConfigurationViewModel(Guid deviceUID, DeviceConfiguration remoteDeviceConfiguration)
		{
			Title = "Сравнение конфигураций";
			ReplaceCommand = new RelayCommand(OnReplace);

			RemoteDeviceConfiguration = remoteDeviceConfiguration;
			RemoteDeviceConfiguration.Update();
			foreach (var device in RemoteDeviceConfiguration.Devices)
			{
				device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
				if (device.Driver == null)
				{
					;
				}
			}
			RemoteDeviceConfiguration.Reorder();
			RemoteDeviceConfiguration.InvalidateConfiguration();
			RemoteDeviceConfiguration.UpdateCrossReferences();

			LocalRootDevice = FiresecManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			RemoteRootDevice = RemoteDeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);

			LocalRootClone = (Device)FiresecManager.Devices.FirstOrDefault(x => x.UID == deviceUID).Clone();
			RemoteRootClone = (Device)RemoteDeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID).Clone();

			LocalRootClone.Children = new List<Device>();
			if (LocalRootDevice.Children != null)
				foreach (var children in LocalRootDevice.Children)
				{
					var childrenClone = (Device)children.Clone();
					childrenClone.DeviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration;
					LocalRootClone.Children.Add(childrenClone);
					if (children.Children != null)
					{
						var localchch =
							LocalRootClone.Children.FirstOrDefault(
								x =>
								((x.PresentationName == children.PresentationName) &&
								 (x.AddressFullPath == children.AddressFullPath)));
						localchch.Children = new List<Device>();
						foreach (var chch in children.Children)
						{
							var chchClone = (Device)chch.Clone();
							chchClone.DeviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration;
							localchch.Children.Add(chchClone);
						}
					}
				}

			RemoteRootClone.Children = new List<Device>();
			if (RemoteRootDevice.Children != null)
				foreach (var children in RemoteRootDevice.Children)
				{
					var childrenClone = (Device)children.Clone();
					childrenClone.DeviceConfiguration = RemoteDeviceConfiguration;
					RemoteRootClone.Children.Add(childrenClone);
					if (children.Children != null)
					{
						var remotechch =
							RemoteRootClone.Children.FirstOrDefault(
								x =>
								((x.PresentationName == children.PresentationName) &&
								 (x.AddressFullPath == children.AddressFullPath)));
						remotechch.Children = new List<Device>();
						foreach (var chch in children.Children)
						{
							var chchClone = (Device)chch.Clone();
							chchClone.DeviceConfiguration = RemoteDeviceConfiguration;
							remotechch.Children.Add(chchClone);
						}
					}
				}

			IntoLocalDevice(LocalRootDevice, RemoteRootClone);
			IntoRemoteDevice(RemoteRootDevice, LocalRootClone);

			Sort(LocalRootClone);
			Sort(RemoteRootClone);

			LocalDevices = new DeviceTreeViewModel(LocalRootClone);
			RemoteDevices = new DeviceTreeViewModel(RemoteRootClone);
		}

		void IntoLocalDevice(Device localRootDevice, Device remoteRootDevice)
		{
			remoteRootDevice.DeviceConfiguration = RemoteDeviceConfiguration;
			foreach (var localDevice in localRootDevice.Children)
			{
				var remoteAndLocal =
					remoteRootDevice.Children.FirstOrDefault(x => (x.Driver.ShortName == localDevice.Driver.ShortName) && (x.AddressFullPath == localDevice.AddressFullPath));
				if (remoteAndLocal == null)
				{
					var remoteDevice = (Device)localDevice.Clone();
					remoteDevice.Children = new List<Device>();
					remoteDevice.HasMissingDifferences = true;
					remoteAndLocal = remoteDevice;
					remoteAndLocal.DeviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration;
					remoteRootDevice.Children.Add(remoteDevice);
				}
				else
				{
					if (remoteAndLocal.Zone == null && localDevice.Zone != null)
					{
						remoteAndLocal.Zone = localDevice.Zone;
						remoteAndLocal.HasDifferences = true;
						remoteAndLocal.DeviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration;
					}
					else if (remoteAndLocal.ZonesInLogic.Count == 0 && localDevice.ZonesInLogic.Count != 0)
					{
						remoteAndLocal.ZonesInLogic = localDevice.ZonesInLogic;
						remoteAndLocal.ZoneLogic = localDevice.ZoneLogic;
						remoteAndLocal.HasDifferences = true;
						remoteAndLocal.DeviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration;
					}
					else
					{
						remoteAndLocal.HasDifferences = false;
						remoteAndLocal.DeviceConfiguration = RemoteDeviceConfiguration;
					}
				}

				if ((localDevice.Children != null) && (localDevice.Children.Count > 0))
				{
					IntoLocalDevice(localDevice, remoteAndLocal);
				}
			}
		}

		void IntoRemoteDevice(Device remoteRootDevice, Device localRootDevice)
		{
			localRootDevice.DeviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration;
			foreach (var remote in remoteRootDevice.Children)
			{
				var localAndRemote = localRootDevice.Children.FirstOrDefault(x => (x.Driver.ShortName == remote.Driver.ShortName) && (x.AddressFullPath == remote.AddressFullPath));
				if (localAndRemote == null)
				{
					var local = (Device)remote.Clone();
					local.Children = new List<Device>();
					local.HasMissingDifferences = true;
					localAndRemote = local;
					localAndRemote.DeviceConfiguration = RemoteDeviceConfiguration;
					localRootDevice.Children.Add(local);
				}
				else
				{
					if (localAndRemote.Zone == null && remote.Zone != null)
					{
						localAndRemote.HasDifferences = true;
						localAndRemote.DeviceConfiguration = RemoteDeviceConfiguration;
					}
					if (localAndRemote.Zone != null && remote.Zone != null && localAndRemote.Zone.PresentationName != remote.Zone.PresentationName)
					{
						localAndRemote.HasDifferences = true;
						localAndRemote.DeviceConfiguration = RemoteDeviceConfiguration;
					}
					else if (localAndRemote.ZonesInLogic.Count == 0 && remote.ZonesInLogic.Count != 0)
					{
						localAndRemote.HasDifferences = true;
						localAndRemote.DeviceConfiguration = RemoteDeviceConfiguration;
					}
					else
					{
						localAndRemote.HasDifferences = false;
						localAndRemote.DeviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration;
					}
				}
				if ((remote.Children != null) && (remote.Children.Count > 0))
				{
					IntoRemoteDevice(remote, localAndRemote);
				}
			}
		}

		void Sort(Device device)
		{
			if (device.Children != null)
			{
				device.Children = device.Children.OrderByDescending(x => x.IntAddress).ToList();
			}
			foreach (var child in device.Children)
			{
				if (child.Children != null)
					Sort(child);
			}
		}

		public DeviceTreeViewModel LocalDevices { get; private set; }
		public DeviceTreeViewModel RemoteDevices { get; private set; }

		public RelayCommand ReplaceCommand { get; private set; }
		void OnReplace()
		{
			try
			{
				LocalRootDevice.Children = new List<Device>();
				LocalRootDevice.Children = RemoteRootDevice.Children;

				var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device.UID == LocalRootDevice.UID);
				if (deviceViewModel == null)
				{
					Logger.Error("DeviceConfigurationViewModel.OnReplace deviceViewModel = null");
					return;
				}

				deviceViewModel.CollapseChildren();
				deviceViewModel.Nodes.Clear();

				foreach (var device in LocalRootDevice.Children)
				{
					BuildZones(device);
					DevicesViewModel.Current.AddDevice(device, deviceViewModel);
				}

				deviceViewModel.ExpandChildren();
				FiresecManager.FiresecConfiguration.DeviceConfiguration.Update();
				ServiceFactory.SaveService.FSChanged = true;
				DevicesViewModel.UpdateGuardVisibility();
				FiresecManager.FiresecConfiguration.UpdateConfiguration();
				Close(true);
			}
			catch (Exception e)
			{
				Logger.Error(e, "DeviceConfigurationViewModel.OnReplace");
			}
		}

		void BuildZones(Device device)
		{
			if (device.Zone != null)
			{
				if (!FiresecManager.Zones.Any(x => (x.No == device.Zone.No)))
				{
					FiresecManager.FiresecConfiguration.AddZone(device.Zone);
					ZonesViewModel.Current.Zones.Add(new ZoneViewModel(device.Zone));
				}
				device.Zone = FiresecManager.Zones.FirstOrDefault(x => x.No == device.Zone.No);
				FiresecManager.FiresecConfiguration.AddDeviceToZone(device, device.Zone);
			}

			if ((device.ZonesInLogic != null) && (device.ZonesInLogic.Count > 0))
			{
				var newZonesInLogic = new List<Zone>();
				foreach (var zoneInLogic in device.ZonesInLogic)
				{
					if (!FiresecManager.Zones.Any(x => (x.No == zoneInLogic.No)))
					{
						FiresecManager.FiresecConfiguration.AddZone(zoneInLogic);
						ZonesViewModel.Current.Zones.Add(new ZoneViewModel(zoneInLogic));
					}
					else
					{
						newZonesInLogic.Add(FiresecManager.Zones.FirstOrDefault(x => (x.No == zoneInLogic.No)));
					}
				}
				device.ZonesInLogic = newZonesInLogic;

				foreach (var clause in device.ZoneLogic.Clauses)
				{
					clause.ZoneUIDs = new List<Guid>();
					foreach (var zone in clause.Zones)
					{
						var newZone = device.ZonesInLogic.FirstOrDefault(x => x.No == zone.No);
						if (newZone != null)
						{
							zone.UID = newZone.UID;
							clause.ZoneUIDs.Add(newZone.UID);
						}
					}
				}
			}

			if (device.Children != null)
				foreach (var child in device.Children)
				{
					BuildZones(child);
				}
		}
	}
}