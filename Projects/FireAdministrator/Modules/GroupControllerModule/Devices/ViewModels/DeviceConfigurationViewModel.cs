using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		public DeviceConfigurationViewModel(XDeviceConfiguration localConfiguration, XDeviceConfiguration remoteConfiguration)
		{
			Title = "Конфигурация устройств";
			LocalDevice = localConfiguration.Devices.FirstOrDefault(x => x.Driver.DriverType == XDriverType.GK);
			RemoteDevice = remoteConfiguration.Devices.FirstOrDefault(x => x.Driver.DriverType == XDriverType.GK);

			var localDeviceClone = (XDevice) LocalDevice.Clone();
			var remoteDeviceClone = (XDevice) RemoteDevice.Clone();

			//var localAndRootDevices = CompareDevice(localDeviceClone, remoteDeviceClone);
			//localDeviceClone.Children = localAndRootDevices[0];
			//remoteDeviceClone.Children = localAndRootDevices[1];

			LocalDeviceViewModel = new DeviceTreeViewModel(localDeviceClone, localConfiguration.Zones, localConfiguration.Directions);
			RemoteDeviceViewModel = new DeviceTreeViewModel(remoteDeviceClone, remoteConfiguration.Zones, remoteConfiguration.Directions);

			DeviceTreeViewModel.CompareTrees(LocalDeviceViewModel, RemoteDeviceViewModel);
			ChangeCommand = new RelayCommand(OnChange);
		}
	
		//static List<List<XDevice>> CompareDevice(XDevice device1, XDevice device2)
		//{
		//    var localAndRootDevices = new List<List<XDevice>>();
		//    var localDevices = XManager.GetAllDeviceChildren(device1);
		//    var remoteDevices = XManager.GetAllDeviceChildren(device2);

		//    var devices = new List<XDevice>(localDevices);
		//    foreach (var device in remoteDevices)
		//    {
		//        if (!device.Driver.HasAddress)
		//            continue;
		//        if (!devices.Any(x => (x.PresentationAddressAndDriver == device.PresentationAddressAndDriver)))
		//            devices.Add((XDevice)device.Clone());
		//    }

		//    foreach (var device in devices)
		//    {
		//        if (!device.Driver.HasAddress)
		//            continue;
		//        if (!localDevices.Any(x => (x.PresentationAddressAndDriver == device.PresentationAddressAndDriver)))
		//        {
		//            var tempDevice = (XDevice)device.Clone();
		//            tempDevice.HasMissingDifferences = true;
		//            localDevices.Add(tempDevice);
		//        }
		//        if (!remoteDevices.Any(x => (x.PresentationAddressAndDriver == device.PresentationAddressAndDriver)))
		//        {
		//            var tempDevice = (XDevice)device.Clone();
		//            tempDevice.HasMissingDifferences = true;
		//            remoteDevices.Add(tempDevice);
		//        }
		//    }
		//    localAndRootDevices.Add(localDevices);
		//    localAndRootDevices.Add(remoteDevices);
		//    return localAndRootDevices;
		//}
		//static void CompareDevices(ObservableCollection<DeviceViewModel> devices1, ObservableCollection<DeviceViewModel> devices2)
		//{
		//    var devices = new List<DeviceViewModel>(devices1);
		//    foreach (var device in devices2)
		//    {
		//        if (!device.Driver.HasAddress)
		//            continue;
		//        if (!devices.Any(x => (x.Device.PresentationAddressAndDriver == device.Device.PresentationAddressAndDriver)))
		//            devices.Add(device);
		//    }

		//    foreach (var device in devices)
		//    {
		//        if (!device.Driver.HasAddress)
		//            continue;
		//        if (!devices1.Any(x => (x.Device.PresentationAddressAndDriver == device.Device.PresentationAddressAndDriver)))
		//        {
		//            var tempDevice = new DeviceViewModel((XDevice)device.Device.Clone());
		//            tempDevice.Device.HasMissingDifferences = true;
		//            devices1.Add(tempDevice);
		//        }
		//        if (!devices2.Any(x => (x.Device.PresentationAddressAndDriver == device.Device.PresentationAddressAndDriver)))
		//        {
		//            var tempDevice = new DeviceViewModel((XDevice)device.Device.Clone());
		//            tempDevice.Device.HasMissingDifferences = true;
		//            devices2.Add(tempDevice);
		//        }
		//    }
		//}
		//void Sort(XDevice device)
		//{
		//    if ((device.Children != null)&&(device.Children.Count != 0))
		//    {
		//        device.Children = device.Children.OrderByDescending(x => x.PresentationDriverAndAddress).ToList();
		//    }
		//    foreach (var child in device.Children)
		//    {
		//        if ((child.Children != null)&&(child.Children.Count != 0))
		//            Sort(child);
		//    }
		//}

		public DeviceTreeViewModel LocalDeviceViewModel { get; set; }
		public DeviceTreeViewModel RemoteDeviceViewModel { get; set; }

		public RelayCommand ChangeCommand { get; private set; }
		void OnChange()
		{
			RemoteDevice.UID = LocalDevice.UID;
			LocalDevice = RemoteDevice;
			//RemoteDevices.RemoveAll(x => x.Driver.IsKauOrRSR2Kau);
			//KauDevice.Children = new List<XDevice>();
			//KauDevice.Children.AddRange(RemoteDevices);
			ServiceFactory.SaveService.GKChanged = true;
			Close(true);
		}
	}
}