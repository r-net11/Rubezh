using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.Plans.Designer;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.Plans.ViewModels
{
	public class DevicePropertiesViewModel : SaveCancelDialogViewModel
	{
		private DevicesViewModel _devicesViewModel;
		private ElementDevice _elementDevice;

		public DevicePropertiesViewModel(DevicesViewModel devicesViewModel, ElementDevice elementDevice)
		{
			Title = "Свойства фигуры: Устройство";
			_elementDevice = elementDevice;
			_devicesViewModel = devicesViewModel;

			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in FiresecManager.Devices)
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
					parent.Children.Add(device);
				}
			}

			if (Devices.Count > 0)
			{
				CollapseChild(Devices[0]);
				ExpandChild(Devices[0]);
			}

			SelectedDriver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == _elementDevice.AlternativeDriverUID);
			Select(elementDevice.DeviceUID);
		}

		#region DeviceSelection
		public List<DeviceViewModel> AllDevices;

		public void FillAllDevices()
		{
			AllDevices = new List<DeviceViewModel>();
			AddChildPlainDevices(Devices[0]);
		}

		void AddChildPlainDevices(DeviceViewModel parentViewModel)
		{
			AllDevices.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
			{
				AddChildPlainDevices(childViewModel);
			}
		}

		public void Select(Guid deviceUID)
		{
			FillAllDevices();

			var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceViewModel != null)
			{
				deviceViewModel.ExpantToThis();
			}
			SelectedDevice = deviceViewModel;
		}
		#endregion

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");
				UpdateAvailableDriver();
			}
		}

		void CollapseChild(DeviceViewModel parentDeviceViewModel)
		{
			parentDeviceViewModel.IsExpanded = false;

			foreach (var deviceViewModel in parentDeviceViewModel.Children)
			{
				CollapseChild(deviceViewModel);
			}
		}

		void ExpandChild(DeviceViewModel parentDeviceViewModel)
		{
			if (parentDeviceViewModel.Device.Driver.Category != DeviceCategoryType.Device)
			{
				parentDeviceViewModel.IsExpanded = true;
				foreach (var deviceViewModel in parentDeviceViewModel.Children)
				{
					ExpandChild(deviceViewModel);
				}
			}
		}

		ObservableCollection<Driver> _availableDrivers;
		public ObservableCollection<Driver> AvailableDrivers
		{
			get { return _availableDrivers; }
			set
			{
				_availableDrivers = value;
				OnPropertyChanged("AvailableDrivers");
			}
		}

		Driver _selectedDriver;
		public Driver SelectedDriver
		{
			get { return _selectedDriver; }
			set
			{
				_selectedDriver = value;
				OnPropertyChanged("SelectedDriver");
			}
		}

		bool _canChangeDriver;
		public bool CanChangeDriver
		{
			get { return _canChangeDriver; }
			set
			{
				_canChangeDriver = value;
				OnPropertyChanged("CanChangeDriver");
			}
		}

		void UpdateAvailableDriver()
		{
			AvailableDrivers = new ObservableCollection<Driver>();
			CanChangeDriver = false;
			if (SelectedDevice != null)
			{
				if (SelectedDevice.Driver.DriverType == DriverType.AMP_4)
				{
					AvailableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AMP_4));
					AvailableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.HeatDetector));
					CanChangeDriver = true;
				}
			}
			if (SelectedDriver != null)
				SelectedDriver = AvailableDrivers.FirstOrDefault(x => x.UID == SelectedDriver.UID);
		}

		protected override bool Save()
		{
			Guid deviceUID = _elementDevice.DeviceUID;
			Helper.SetDevice(_elementDevice, SelectedDevice.Device);
			if (deviceUID != _elementDevice.DeviceUID)
				Update(deviceUID);
			Update(_elementDevice.DeviceUID);
			_devicesViewModel.Select(_elementDevice.DeviceUID);
			if (SelectedDriver != null)
				_elementDevice.AlternativeDriverUID = SelectedDriver.UID;
			else
				_elementDevice.AlternativeDriverUID = Guid.Empty;
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