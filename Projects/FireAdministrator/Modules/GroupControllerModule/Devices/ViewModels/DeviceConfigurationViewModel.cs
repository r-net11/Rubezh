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
		public DeviceConfigurationViewModel(XDeviceConfiguration localConfiguration, XDeviceConfiguration remoteConfiguration)
		{
			Title = "Конфигурация устройств";
			LocalConfiguration = localConfiguration;
			RemoteConfiguration = remoteConfiguration;

			LocalDevice = localConfiguration.Devices.FirstOrDefault(x => x.Driver.DriverType == XDriverType.GK);
			RemoteDevice = remoteConfiguration.Devices.FirstOrDefault(x => x.Driver.DriverType == XDriverType.GK);
			RemoteDevice.Properties.Add(LocalDevice.Properties.FirstOrDefault(x => x.Name == "IPAddress")); // Клонируем IP адрес

			var localDeviceClone = (XDevice) LocalDevice.Clone();
			var remoteDeviceClone = (XDevice) RemoteDevice.Clone();

			LocalDeviceViewModel = new DeviceTreeViewModel(localDeviceClone, localConfiguration.Zones, localConfiguration.Directions);
			RemoteDeviceViewModel = new DeviceTreeViewModel(remoteDeviceClone, remoteConfiguration.Zones, remoteConfiguration.Directions);

			DeviceTreeViewModel.CompareTrees(LocalDeviceViewModel, RemoteDeviceViewModel);
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
			LocalConfiguration.Zones = RemoteConfiguration.Zones;
			LocalConfiguration.Directions = RemoteConfiguration.Directions;
			ServiceFactory.SaveService.GKChanged = true;
			Close(true);
		}
	}
}