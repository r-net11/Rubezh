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
	public class XDevicePropertiesViewModel : SaveCancelDialogViewModel
	{
		private ElementXDevice _elementXDevice;
		private XDevicesViewModel _xdevicesViewModel;

		public XDevicePropertiesViewModel(XDevicesViewModel xdevicesViewModel, ElementXDevice elementDevice)
		{
			Title = "Свойства фигуры: Устройство ГК";
			_elementXDevice = elementDevice;
			_xdevicesViewModel = xdevicesViewModel;

			Devices = new ObservableCollection<DeviceViewModel>();
			BuildTree();

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
			var xRootDevice = XManager.DeviceConfiguration.RootDevice;
			AddDevice(xRootDevice, null);
		}
		public DeviceViewModel AddDevice(XDevice xDevice, DeviceViewModel parentDeviceViewModel)
		{
			var xDeviceViewModel = new DeviceViewModel(xDevice, Devices);
			xDeviceViewModel.Parent = parentDeviceViewModel;

			var indexOf = Devices.IndexOf(parentDeviceViewModel);
			Devices.Insert(indexOf + 1, xDeviceViewModel);

			if (xDevice != null)
				foreach (var childDevice in xDevice.Children)
				{
					var childDeviceViewModel = AddDevice(childDevice, xDeviceViewModel);
					xDeviceViewModel.Children.Add(childDeviceViewModel);
				}

			return xDeviceViewModel;
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
				deviceViewModel.ExpantToThis();
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
			var device = _xdevicesViewModel.Devices.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (device != null)
				device.Update();
		}
	}
}
