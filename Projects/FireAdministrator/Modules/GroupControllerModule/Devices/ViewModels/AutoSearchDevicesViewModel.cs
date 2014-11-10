using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;

namespace GKModule.ViewModels
{
	public class AutoSearchDevicesViewModel : DialogViewModel
	{
		public AutoSearchDevicesViewModel(GKDeviceConfiguration deviceConfiguration)
		{
			Title = "Устройства, найденные в результате автопоиска";
			deviceConfiguration.UpdateConfiguration();

			RootDevice = AddDeviceInternal(deviceConfiguration.RootDevice, null);
			if (SelectedDevice != null)
				SelectedDevice.ExpandToThis();
		}
		private DeviceViewModel AddDeviceInternal(GKDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);

			if (deviceViewModel.Device.DriverType == GKDriverType.RSR2_KAU_Shleif)
				deviceViewModel.ExpandToThis();

			return deviceViewModel;
		}

		private DeviceViewModel _rootDevice;
		public DeviceViewModel RootDevice
		{
			get { return _rootDevice; }
			private set
			{
				_rootDevice = value;
				OnPropertyChanged(() => RootDevice);
				OnPropertyChanged(() => RootDevices);
			}
		}
		public DeviceViewModel[] RootDevices
		{
			get { return new DeviceViewModel[] { RootDevice }; }
		}

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}
	}
}