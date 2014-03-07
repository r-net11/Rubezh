using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Plans.Designer;
using GKModule.ViewModels;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.Plans.ViewModels
{
	public class DevicePropertiesViewModel : SaveCancelDialogViewModel
	{
		private ElementXDevice _elementXDevice;
		private DevicesViewModel _devicesViewModel;

		public DevicePropertiesViewModel(DevicesViewModel devicesViewModel, ElementXDevice elementDevice)
		{
			Title = "Свойства фигуры: Устройство ГК";
			_elementXDevice = elementDevice;
			_devicesViewModel = devicesViewModel;

			RootDevice = AddDeviceInternal(XManager.DeviceConfiguration.RootDevice, null);
			if (SelectedDevice != null)
				SelectedDevice.ExpandToThis();
		}
		private DeviceViewModel AddDeviceInternal(XDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);
			if (device.BaseUID == _elementXDevice.XDeviceUID)
				SelectedDevice = deviceViewModel;
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
				OnPropertyChanged("SelectedDevice");
			}
		}

		protected override bool Save()
		{
			Guid deviceUID = _elementXDevice.XDeviceUID;
			Helper.SetXDevice(_elementXDevice, SelectedDevice.Device);

			if (deviceUID != _elementXDevice.XDeviceUID)
				Update(deviceUID);
			_devicesViewModel.SelectedDevice = Update(_elementXDevice.XDeviceUID);
			return base.Save();
		}
		private DeviceViewModel Update(Guid deviceUID)
		{
			var device = _devicesViewModel.AllDevices.FirstOrDefault(x => x.Device.BaseUID == deviceUID);
			if (device != null)
				device.Update();
			return device;
		}
	}
}