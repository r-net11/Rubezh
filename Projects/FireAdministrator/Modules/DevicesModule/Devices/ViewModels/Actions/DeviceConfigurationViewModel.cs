using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Common;

namespace DevicesModule.ViewModels
{
	public class DeviceConfigurationViewModel : DialogViewModel
	{
		DeviceConfiguration RemoteDeviceConfiguration;
		Device LocalRootDevice;
		Device RemoteRootDevice;

		public DeviceConfigurationViewModel(Guid deviceUID, DeviceConfiguration remoteDeviceConfiguration)
		{
			Title = "Сравнение конфигураций";
			ReplaceCommand = new RelayCommand(OnReplace);

			RemoteDeviceConfiguration = remoteDeviceConfiguration;
			RemoteDeviceConfiguration.Reorder();
			RemoteDeviceConfiguration.Update();
			RemoteDeviceConfiguration.InvalidateConfiguration();
			RemoteDeviceConfiguration.UpdateCrossReferences();

			foreach (var device in RemoteDeviceConfiguration.Devices)
			{
				device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
			}

			LocalRootDevice = FiresecManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			RemoteRootDevice = RemoteDeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			var UnionRootDevice = (Device)FiresecManager.Devices.FirstOrDefault(x => x.UID == deviceUID).Clone();
			UnionRootDevice.Children = new List<Device>();
			foreach (var localChild in LocalRootDevice.Children)
			{
				localChild.IsLocal = true;
				localChild.IsRemote = false;
				UnionRootDevice.Children.Add(localChild);
			}

			foreach (var remoteChild in RemoteRootDevice.Children)
			{
				remoteChild.IsRemote = true;
				remoteChild.IsLocal = false;
				if (remoteChild != null)
				{
					if (remoteChild.Zone != null)
					{
						var zone = FiresecManager.Zones.FirstOrDefault(x => x.No == remoteChild.Zone.No);
						if (zone != null)
						{
							if (zone.ZoneType == remoteChild.Zone.ZoneType)
								remoteChild.Zone = zone;
							else
								remoteChild.Zone = null;
						}
					}
				}
				var localAndRemote = LocalRootDevice.Children.FirstOrDefault(x => x.PresentationAddressAndName == remoteChild.PresentationAddressAndName);
				if (localAndRemote != null)
				{
					var unionRootDevice = UnionRootDevice.Children.FirstOrDefault(x => x.PresentationAddressAndName == localAndRemote.PresentationAddressAndName);
                    unionRootDevice.IsRemote = true;
                    if ((localAndRemote.Zone == null) && (remoteChild.Zone != null))
                        unionRootDevice.Zone = remoteChild.Zone;
                    if (((localAndRemote.ZonesInLogic != null)&&(localAndRemote.ZonesInLogic.Count == 0)) && (remoteChild.ZonesInLogic != null))
                    {
                        unionRootDevice.ZonesInLogic = remoteChild.ZonesInLogic;
                        unionRootDevice.ZoneLogic = remoteChild.ZoneLogic;
                    }
				}
				else
				{
					UnionRootDevice.Children.Add(remoteChild);
				}
			}

			LocalDevices = new DeviceTreeViewModel(UnionRootDevice, FiresecManager.FiresecConfiguration.DeviceConfiguration, false);
			RemoteDevices = new DeviceTreeViewModel(UnionRootDevice, RemoteDeviceConfiguration, true);
		}

		public DeviceTreeViewModel LocalDevices { get; private set; }
		public DeviceTreeViewModel RemoteDevices { get; private set; }

		public RelayCommand ReplaceCommand { get; private set; }
		void OnReplace()
		{
			LocalRootDevice.Parent.Children.Remove(LocalRootDevice);
			LocalRootDevice.Parent.Children.Add(RemoteRootDevice);
			RemoteRootDevice.Parent = LocalRootDevice.Parent;
			var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device.UID == LocalRootDevice.UID);
			if (deviceViewModel == null)
			{
				Logger.Error("DeviceConfigurationViewModel.OnReplace deviceViewModel = null");
				return;
			}
			deviceViewModel.CollapseChildren();
			deviceViewModel.Children.Clear();
			foreach (var device in RemoteRootDevice.Children)
			{
				DevicesViewModel.Current.AddDevice(device, deviceViewModel);
                if (device.Zone != null)
                {
                    if (FiresecManager.Zones.Any(x => (x.No == device.Zone.No) && (x.ZoneType == device.Zone.ZoneType)))
                        device.Zone = FiresecManager.Zones.FirstOrDefault(x => x.No == device.Zone.No);
                    else
                    {
                        FiresecManager.FiresecConfiguration.AddZone(device.Zone);
                        ZonesViewModel.Current.Zones.Add(new ZoneViewModel(device.Zone));
                    }
                }

                if ((device.ZonesInLogic != null) && (device.ZonesInLogic.Count > 0))
                {
                    var localDevice = LocalRootDevice.Children.FirstOrDefault(x => x.AddressFullPath == device.AddressFullPath);
                    if ((localDevice != null) && (localDevice.ZonesInLogic != null) && (localDevice.ZonesInLogic.Count > 0))
                    {
                        device.ZonesInLogic = localDevice.ZonesInLogic;
                        device.ZoneLogic = localDevice.ZoneLogic;
                    }
                    else
                    {
                        List<Zone> tempZonesInLogic = new List<Zone>();
                        foreach (var zoneInLogic in device.ZonesInLogic)
                        {
                            if (
                                FiresecManager.Zones.Any(
                                    x => (x.No == zoneInLogic.No) && (x.ZoneType == zoneInLogic.ZoneType)))
                                tempZonesInLogic.Add(FiresecManager.Zones.FirstOrDefault(x => x.No == zoneInLogic.No));
                            else
                            {
                                FiresecManager.FiresecConfiguration.AddZone(zoneInLogic);
                                ZonesViewModel.Current.Zones.Add(new ZoneViewModel(zoneInLogic));
                            }
                        }
                        device.ZonesInLogic = tempZonesInLogic;
                        device.ZoneLogic.Clauses[0].Zones = tempZonesInLogic;
                        device.ZoneLogic.Clauses[0].ZoneUIDs = new List<Guid>();
                        foreach (var tempZoneInLogic in tempZonesInLogic)
                        {
                            device.ZoneLogic.Clauses[0].ZoneUIDs.Add(tempZoneInLogic.UID);
                        }
                    }
                }

			}
			deviceViewModel.ExpandChildren();

			FiresecManager.FiresecConfiguration.DeviceConfiguration.Update();
			ServiceFactory.SaveService.FSChanged = true;
			DevicesViewModel.UpdateGuardVisibility();
			Close(true);
		}
	}
}