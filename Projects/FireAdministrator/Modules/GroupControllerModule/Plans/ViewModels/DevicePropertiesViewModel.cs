using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in XManager.Devices)
			{
				var deviceViewModel = new DeviceViewModel(device);
				deviceViewModel.IsExpanded = true;
				Devices.Add(deviceViewModel);
			}
			foreach (var device in Devices)
			{
				if (device.Device.Parent != null)
				{
					var parent = Devices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
					parent.AddChild(device);
				}
			}
			//BuildTree();

			if (Devices.Count > 0)
			{
				CollapseChild(Devices[0]);
				ExpandChild(Devices[0]);
			}

			Select(elementDevice.XDeviceUID);
		}

		public List<DeviceViewModel> AllDevices;
		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

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

		private void BuildTree()
		{
			var rootDevice = XManager.DeviceConfiguration.RootDevice;
			AddDevice(rootDevice, null);
		}
		public DeviceViewModel AddDevice(XDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			parentDeviceViewModel.AddChild(deviceViewModel);

			var indexOf = Devices.IndexOf(parentDeviceViewModel);
			Devices.Insert(indexOf + 1, deviceViewModel);

			if (device != null)
				foreach (var childDevice in device.Children)
				{
					var childDeviceViewModel = AddDevice(childDevice, deviceViewModel);
					deviceViewModel.AddChild(childDeviceViewModel);
				}

			return deviceViewModel;
		}

		public void FillAllDevices()
		{
			AllDevices = new List<DeviceViewModel>();
			AddChildPlainDevices(Devices[0]);
		}
		private void AddChildPlainDevices(DeviceViewModel parentViewModel)
		{
			AllDevices.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainDevices(childViewModel);
		}
		public void Select(Guid deviceUID)
		{
			FillAllDevices();

			var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceViewModel != null)
				deviceViewModel.ExpandToThis();
			SelectedDevice = deviceViewModel;
		}

		private void CollapseChild(DeviceViewModel parentDeviceViewModel)
		{
			parentDeviceViewModel.IsExpanded = false;
			foreach (var deviceViewModel in parentDeviceViewModel.Children)
				CollapseChild(deviceViewModel);
		}
		private void ExpandChild(DeviceViewModel parentDeviceViewModel)
		{
			parentDeviceViewModel.IsExpanded = true;
			foreach (var deviceViewModel in parentDeviceViewModel.Children)
				ExpandChild(deviceViewModel);
		}

		protected override bool Save()
		{
			Guid deviceUID = _elementXDevice.XDeviceUID;
			Helper.SetXDevice(_elementXDevice, SelectedDevice.Device);

			if (deviceUID != _elementXDevice.XDeviceUID)
				Update(deviceUID);
			Update(_elementXDevice.XDeviceUID);
			return base.Save();
		}
		private void Update(Guid deviceUID)
		{
			var device = _devicesViewModel.AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (device != null)
				device.Update();
		}
	}
}