using System;
using System.Collections.Generic;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Linq;

namespace GKModule.ViewModels
{
	public class DeviceConfigurationViewModel : DialogViewModel
	{
		private XDevice LocalDevice { get; set; }
		private XDevice RemoteDevice { get; set; }
		private XDeviceConfiguration LocalConfiguration { get; set; }
		private XDeviceConfiguration RemoteConfiguration { get; set; }
		public DeviceConfigurationViewModel(XDeviceConfiguration localConfiguration, XDeviceConfiguration remoteConfiguration, XDevice device)
		{
			Title = "Конфигурация устройств";
			LocalConfiguration = localConfiguration;
			RemoteConfiguration = remoteConfiguration;

			LocalDevice = localConfiguration.Devices.FirstOrDefault(x => (x.Driver.DriverType == device.Driver.DriverType)&&(x.Address == device.Address));
			RemoteDevice = remoteConfiguration.Devices.FirstOrDefault(x => (x.Driver.DriverType == device.Driver.DriverType) && (x.Address == device.Address));

			var localDeviceClone = (XDevice) LocalDevice.Clone();
			var remoteDeviceClone = (XDevice) RemoteDevice.Clone();

			if (device.Driver.DriverType == XDriverType.GK)
			{
				LocalDeviceViewModel = new DeviceTreeViewModel(localDeviceClone, localConfiguration.Zones,
															   localConfiguration.Directions);
				RemoteDeviceViewModel = new DeviceTreeViewModel(remoteDeviceClone, remoteConfiguration.Zones,
																remoteConfiguration.Directions);
			}
			else
			{
				LocalDeviceViewModel = new DeviceTreeViewModel(localDeviceClone, null, null);
				RemoteDeviceViewModel = new DeviceTreeViewModel(remoteDeviceClone, null, null);
			}
            var compareDevices = DeviceTreeViewModel.CompareTrees(LocalDeviceViewModel.Devices, RemoteDeviceViewModel.Devices, device.Driver.DriverType);
			LocalDeviceViewModel.Devices = compareDevices[0];
			RemoteDeviceViewModel.Devices = compareDevices[1];
            if (device.Driver.DriverType == XDriverType.GK)
            {
                var compareZones = DeviceTreeViewModel.CompareTrees(LocalDeviceViewModel.Zones, RemoteDeviceViewModel.Zones, device.Driver.DriverType);
                LocalDeviceViewModel.Zones = compareZones[0];
                RemoteDeviceViewModel.Zones = compareZones[1];
                var compareDirections = DeviceTreeViewModel.CompareTrees(LocalDeviceViewModel.Directions, RemoteDeviceViewModel.Directions, device.Driver.DriverType);
                LocalDeviceViewModel.Directions = compareDirections[0];
                RemoteDeviceViewModel.Directions = compareDirections[1];
            }
			ChangeCommand = new RelayCommand(OnChange);
			NextDifferenceCommand = new RelayCommand(OnNextDifference);
		}
		public DeviceTreeViewModel LocalDeviceViewModel { get; set; }
		public DeviceTreeViewModel RemoteDeviceViewModel { get; set; }
		public RelayCommand ChangeCommand { get; private set; }
		void OnChange()
		{
			RemoteDevice.UID = LocalDevice.UID;
			var rootDevice = LocalConfiguration.Devices.FirstOrDefault(x => x.UID == LocalDevice.Parent.UID);
			rootDevice.Children.Remove(LocalDevice);
			rootDevice.Children.Add(RemoteDevice);
			if(RemoteDevice.Driver.DriverType == XDriverType.GK)
			{
				foreach (var kauChild in RemoteDevice.Children)
				{
					if(kauChild.Driver.IsKauOrRSR2Kau)
						AddShleifs(kauChild);
				}
			}
			if(RemoteDevice.Driver.IsKauOrRSR2Kau)
				AddShleifs(RemoteDevice);
			if (LocalDevice.Driver.DriverType == XDriverType.GK)
			{
				LocalConfiguration.Zones = RemoteConfiguration.Zones;
				LocalConfiguration.Directions = RemoteConfiguration.Directions;
			}
			ServiceFactory.SaveService.GKChanged = true;
			XManager.UpdateConfiguration();
			Close(true);
		}

		public RelayCommand NextDifferenceCommand { get; private set; }
		void OnNextDifference()
		{
			var startindex = LocalDeviceViewModel.SelectedObject != null? LocalDeviceViewModel.Objects.IndexOf(LocalDeviceViewModel.SelectedObject) + 1: 0;
			var endindex = LocalDeviceViewModel.Objects.Count - startindex;
			var newSelected = LocalDeviceViewModel.Objects.ToList().GetRange(startindex, endindex).FirstOrDefault(x => x.HasDifferences);
			if (newSelected != null)
			    LocalDeviceViewModel.SelectedObject = newSelected;
		}
		static void AddShleifs(XDevice device)
		{
			const int shleifsCount = 8;
			var deviceChildren = new List<XDevice>(device.Children);
			device.Children = new List<XDevice>();
			for (int i = 0; i < shleifsCount; i++)
			{
				var shleif = new XDevice();
				shleif.Driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.KAU_Shleif);
				shleif.DriverUID = shleif.Driver.UID;
				shleif.IntAddress = (byte)(i + 1);
				device.Children.Add(shleif);
			}
			foreach (var child in deviceChildren)
			{
				if ((1 <= child.ShleifNo)&&(child.ShleifNo <= 8))
				{
					var shleif = device.Children.FirstOrDefault(x => (x.Driver.DriverType == XDriverType.KAU_Shleif) && (x.IntAddress == child.ShleifNo));
					shleif.Children.Add(child);
					child.Parent = shleif;
				}
			}
		}
	}
}