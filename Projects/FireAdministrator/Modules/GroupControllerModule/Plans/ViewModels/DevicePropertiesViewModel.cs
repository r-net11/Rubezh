using System;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using GKModule.ViewModels;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;

namespace GKModule.Plans.ViewModels
{
	public class DevicePropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementGKDevice _elementGKDevice;
		DevicesViewModel _devicesViewModel;

		public DevicePropertiesViewModel(DevicesViewModel devicesViewModel, ElementGKDevice elementDevice)
		{
			Title = "Свойства фигуры: Устройство ГК";
			_elementGKDevice = elementDevice;
			_devicesViewModel = devicesViewModel;

			RootDevice = AddDeviceInternal(GKManager.DeviceConfiguration.RootDevice, null);
			if (SelectedDevice != null)
				SelectedDevice.ExpandToThis();
		}
		DeviceViewModel AddDeviceInternal(GKDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);
			if (device.UID == _elementGKDevice.DeviceUID)
				SelectedDevice = deviceViewModel;
			return deviceViewModel;
		}

		DeviceViewModel _rootDevice;
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
			Guid deviceUID = _elementGKDevice.DeviceUID;
			GKPlanExtension.Instance.SetItem<GKDevice>(_elementGKDevice, SelectedDevice.Device);

			if (deviceUID != _elementGKDevice.DeviceUID)
				Update(deviceUID);
			_devicesViewModel.SelectedDevice = Update(_elementGKDevice.DeviceUID);
			return base.Save();
		}

		protected override bool CanSave()
		{
			return SelectedDevice != null && SelectedDevice.Driver.IsPlaceable;
		}

		DeviceViewModel Update(Guid deviceUID)
		{
			var device = _devicesViewModel.AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (device != null)
				device.Update();
			return device;
		}
	}
}