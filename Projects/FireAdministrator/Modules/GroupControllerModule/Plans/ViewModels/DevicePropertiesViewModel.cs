using System;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.ViewModels;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.Plans.ViewModels
{
	public class DevicePropertiesViewModel : SaveCancelDialogViewModel
	{
		private ElementGKDevice _elementXDevice;
		private DevicesViewModel _devicesViewModel;

		public DevicePropertiesViewModel(DevicesViewModel devicesViewModel, ElementGKDevice elementDevice)
		{
			Title = "Свойства фигуры: Устройство ГК";
			_elementXDevice = elementDevice;
			_devicesViewModel = devicesViewModel;

			RootDevice = AddDeviceInternal(GKManager.DeviceConfiguration.RootDevice, null);
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
			if (device.UID == _elementXDevice.DeviceUID)
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
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		protected override bool Save()
		{
			Guid deviceUID = _elementXDevice.DeviceUID;
			GKPlanExtension.Instance.SetItem<GKDevice>(_elementXDevice, SelectedDevice.Device);

			if (deviceUID != _elementXDevice.DeviceUID)
				Update(deviceUID);
			_devicesViewModel.SelectedDevice = Update(_elementXDevice.DeviceUID);
			return base.Save();
		}
		private DeviceViewModel Update(Guid deviceUID)
		{
			var device = _devicesViewModel.AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (device != null)
				device.Update();
			return device;
		}
	}
}