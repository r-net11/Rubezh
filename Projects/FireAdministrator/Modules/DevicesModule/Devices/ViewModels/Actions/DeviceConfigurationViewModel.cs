using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

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
				var localAndRemote = LocalRootDevice.Children.FirstOrDefault(x => x.PresentationAddressAndDriver == remoteChild.PresentationAddressAndDriver);
				if (localAndRemote != null)
				{
					UnionRootDevice.Children.FirstOrDefault(
						x => x.PresentationAddressAndDriver == localAndRemote.PresentationAddressAndDriver).IsRemote = true;
				}
				else
				{
					UnionRootDevice.Children.Add(remoteChild);
				}
			}

			LocalDevices = new DeviceTreeViewModel(UnionRootDevice, FiresecManager.FiresecConfiguration.DeviceConfiguration, true);
			RemoteDevices = new DeviceTreeViewModel(UnionRootDevice, RemoteDeviceConfiguration, false);
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
			deviceViewModel.CollapseChildren();
			deviceViewModel.Children.Clear();
			foreach (var device in RemoteRootDevice.Children)
			{
				DevicesViewModel.Current.AddDevice(device, deviceViewModel);
				if (device.Zone == null)
					continue;
				if (FiresecManager.Zones.Any(x => (x.No == device.Zone.No)&&(x.ZoneType == device.Zone.ZoneType)))
					device.Zone = FiresecManager.Zones.FirstOrDefault(x => x.No == device.Zone.No);
				else
				{
					FiresecManager.FiresecConfiguration.AddZone(device.Zone);
					ZonesViewModel.Current.Zones.Add(new ZoneViewModel(device.Zone));
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