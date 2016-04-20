using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.Plans.Designer;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using Devices = DevicesModule.ViewModels;

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
					parent.AddChild(device);
				}
			}

			if (Devices.Count > 0)
			{
				CollapseChild(Devices[0]);
				ExpandChild(Devices[0]);
			}

			SelectedDriver = FiresecManager.DeviceLibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == _elementDevice.AlternativeLibraryDeviceUID);
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
			foreach (DeviceViewModel childViewModel in parentViewModel.Children)
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
				deviceViewModel.ExpandToThis();
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
				OnPropertyChanged(() => SelectedDevice);
				UpdateAvailableDriver();
			}
		}

		void CollapseChild(DeviceViewModel parentDeviceViewModel)
		{
			parentDeviceViewModel.IsExpanded = false;

			foreach (DeviceViewModel deviceViewModel in parentDeviceViewModel.Children)
			{
				CollapseChild(deviceViewModel);
			}
		}

		void ExpandChild(DeviceViewModel parentDeviceViewModel)
		{
			if (parentDeviceViewModel.Device.Driver.Category != DeviceCategoryType.Device)
			{
				parentDeviceViewModel.IsExpanded = true;
				foreach (DeviceViewModel deviceViewModel in parentDeviceViewModel.Children)
				{
					ExpandChild(deviceViewModel);
				}
			}
		}

		ObservableCollection<LibraryDevice> _availableDrivers;
		public ObservableCollection<LibraryDevice> AvailableDrivers
		{
			get { return _availableDrivers; }
			set
			{
				_availableDrivers = value;
				OnPropertyChanged(() => AvailableDrivers);
			}
		}

		LibraryDevice _selectedDriver;
		public LibraryDevice SelectedDriver
		{
			get { return _selectedDriver; }
			set
			{
				_selectedDriver = value;
				OnPropertyChanged(() => SelectedDriver);
			}
		}

		bool _canChangeDriver;
		public bool CanChangeDriver
		{
			get { return _canChangeDriver; }
			set
			{
				_canChangeDriver = value;
				OnPropertyChanged(() => CanChangeDriver);
			}
		}

		void UpdateAvailableDriver()
		{
			AvailableDrivers = new ObservableCollection<LibraryDevice>();
			CanChangeDriver = false;
			if (SelectedDevice != null)
			{
				AvailableDrivers.Add(FiresecManager.DeviceLibraryConfiguration.Devices.FirstOrDefault(x => x.Driver.DriverType == SelectedDevice.Device.Driver.DriverType));
				foreach (var libraryDevice in FiresecManager.DeviceLibraryConfiguration.Devices)
				{
					if (libraryDevice.DriverId == SelectedDevice.Device.DriverUID && libraryDevice.IsAlternative)
					{
						AvailableDrivers.Add(libraryDevice);
						CanChangeDriver = true;
					}
				}
				if (SelectedDevice.Driver.DriverType == DriverType.AMP_4)
				{
					AvailableDrivers.Add(FiresecManager.DeviceLibraryConfiguration.Devices.FirstOrDefault(x => x.Driver.DriverType == DriverType.HeatDetector));
					CanChangeDriver = true;
				}
				if (SelectedDriver != null)
					SelectedDriver = AvailableDrivers.FirstOrDefault(x => x.DriverId == SelectedDriver.DriverId);
			}
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
				_elementDevice.AlternativeLibraryDeviceUID = SelectedDriver.DriverId;
			else
				_elementDevice.AlternativeLibraryDeviceUID = Guid.Empty;
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