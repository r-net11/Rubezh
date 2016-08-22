using System;
using System.Linq;
using Localization.Strazh.ViewModels;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using StrazhModule.Plans.Designer;
using StrazhModule.ViewModels;

namespace StrazhModule.Plans.ViewModels
{
	public class DevicePropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementSKDDevice _elementSKDDevice;
		DevicesViewModel _devicesViewModel;

		public DevicePropertiesViewModel(DevicesViewModel devicesViewModel, ElementSKDDevice elementDevice)
		{
			Title = CommonViewModels.FigureProperties_SKDDevice;
			_elementSKDDevice = elementDevice;
			_devicesViewModel = devicesViewModel;

			RootDevice = AddDeviceInternal(SKDManager.SKDConfiguration.RootDevice, null);
			if (SelectedDevice != null)
				SelectedDevice.ExpandToThis();
		}
		DeviceViewModel AddDeviceInternal(SKDDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				if (childDevice.Driver.IsPlaceable)
				{
					AddDeviceInternal(childDevice, deviceViewModel);
				}
			}
			if (device.UID == _elementSKDDevice.DeviceUID)
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
			Guid deviceUID = _elementSKDDevice.DeviceUID;
			SKDPlanExtension.Instance.SetItem<SKDDevice>(_elementSKDDevice, SelectedDevice.Device);

			if (deviceUID != _elementSKDDevice.DeviceUID)
				Update(deviceUID);
			_devicesViewModel.SelectedDevice = Update(_elementSKDDevice.DeviceUID);
			return base.Save();
		}

		protected override bool CanSave()
		{
			return SelectedDevice.Driver.IsPlaceable;
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