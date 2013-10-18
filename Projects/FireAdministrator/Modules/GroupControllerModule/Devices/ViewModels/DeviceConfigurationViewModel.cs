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
			DeviceTreeViewModel.CompareTrees(LocalDeviceViewModel.Devices, RemoteDeviceViewModel.Devices);
			//LocalDeviceViewModel.Devices = LocalDeviceViewModel.Devices.OrderBy(x => x.ParentPath).ToList().OrderBy(x => x.Address).ToList().OrderBy(x => x.Name).ToList();
			//objects2 = objects2.OrderBy(x => x.ParentPath).ToList().OrderBy(x => x.Address).ToList().OrderBy(x => x.Name).ToList();
			if (device.Driver.DriverType == XDriverType.GK)
			{
				DeviceTreeViewModel.CompareTrees(LocalDeviceViewModel.Zones, RemoteDeviceViewModel.Zones);
				DeviceTreeViewModel.CompareTrees(LocalDeviceViewModel.Directions, RemoteDeviceViewModel.Directions);
			}
			ChangeCommand = new RelayCommand(OnChange);
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
			if (LocalDevice.Driver.DriverType == XDriverType.GK)
			{
				LocalConfiguration.Zones = RemoteConfiguration.Zones;
				LocalConfiguration.Directions = RemoteConfiguration.Directions;
			}
			ServiceFactory.SaveService.GKChanged = true;
			Close(true);
		}
	}
}