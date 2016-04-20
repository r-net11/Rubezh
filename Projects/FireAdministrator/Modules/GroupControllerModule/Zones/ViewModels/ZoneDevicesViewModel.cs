using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class ZoneDevicesViewModel : BaseViewModel
	{
		GKZone Zone;

		public ZoneDevicesViewModel()
		{
			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			Devices = new ObservableCollection<ZoneDeviceViewModel>();
			AvailableDevices = new ObservableCollection<ZoneDeviceViewModel>();
		}

		public void Initialize(GKZone zone)
		{
			Zone = zone;
			var devices = new HashSet<GKDevice>();

			foreach (var device in GKManager.Devices)
			{
				if (device.Driver.HasZone)
				{
					if (device.ZoneUIDs.Contains(Zone.UID) && device.ZoneUIDs.Count != 0)
					{
						devices.Add(device);
					}
				}
			}

			Devices = new ObservableCollection<ZoneDeviceViewModel>();
			foreach (var device in devices)
			{
				var deviceViewModel = new ZoneDeviceViewModel(device)
				{
					IsBold = device.ZoneUIDs.Contains(Zone.UID)
				};
				Devices.Add(deviceViewModel);
			}

			OnPropertyChanged(() => Devices);
			SelectedDevice = Devices.FirstOrDefault();

		}

		public void InitializeAvailableDevice()
		{
			var availableDevices = new HashSet<GKDevice>();
			foreach (var device in GKManager.Devices)
			{
				if (device.Driver.HasZone && device.ZoneUIDs.Count == 0)
				{
					if ((device.IsInMPT && !GlobalSettingsHelper.GlobalSettings.ShowMPTsDevicesForZone) ||
						(device.GuardZones.Count > 0 && !GlobalSettingsHelper.GlobalSettings.ShowGuardZonesDevices) ||
						(device.Door != null && !GlobalSettingsHelper.GlobalSettings.ShowDoorsDevicesForZone))
						continue;

					availableDevices.Add(device);
				}
			}

			AvailableDevices = new ObservableCollection<ZoneDeviceViewModel>();
			foreach (var device in availableDevices)
			{
				var deviceViewModel = new ZoneDeviceViewModel(device)
				{
					IsBold = device.Driver.HasZone
				};

				AvailableDevices.Add(deviceViewModel);
			}

			OnPropertyChanged(() => AvailableDevices);
			SelectedAvailableDevice = AvailableDevices.FirstOrDefault();
		}

		public void Clear()
		{
			Devices.Clear();
			AvailableDevices.Clear();
			SelectedDevice = null;
			SelectedAvailableDevice = null;
		}

		public ObservableCollection<ZoneDeviceViewModel> Devices { get; private set; }

		ZoneDeviceViewModel _selectedDevice;
		public ZoneDeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		public ObservableCollection<ZoneDeviceViewModel> AvailableDevices { get; private set; }

		ZoneDeviceViewModel _selectedAvailableDevice;
		public ZoneDeviceViewModel SelectedAvailableDevice
		{
			get { return _selectedAvailableDevice; }
			set
			{
				_selectedAvailableDevice = value;
				OnPropertyChanged(() => SelectedAvailableDevice);
			}
		}

		public RelayCommand<object> AddCommand { get; private set; }
		public IList SelectedAvailableDevices;
		void OnAdd(object parameter)
		{
			var availableDevicesIndex = AvailableDevices.IndexOf(SelectedAvailableDevice);
			var devicesIndex = Devices.IndexOf(SelectedDevice);

			SelectedAvailableDevices = (IList)parameter;
			var availabledeviceViewModels = new List<ZoneDeviceViewModel>();
			foreach (var availabledevice in SelectedAvailableDevices)
			{
				var availabledeviceViewModel = availabledevice as ZoneDeviceViewModel;
				if (availabledeviceViewModel != null)
					availabledeviceViewModels.Add(availabledeviceViewModel);
			}
			foreach (var availabledeviceViewModel in availabledeviceViewModels)
			{
				Devices.Add(availabledeviceViewModel);
				AvailableDevices.Remove(availabledeviceViewModel);
				GKManager.AddDeviceToZone(availabledeviceViewModel.Device, Zone);
			}
			SelectedDevice = Devices.FirstOrDefault();
			SelectedAvailableDevice = AvailableDevices.FirstOrDefault();

			availableDevicesIndex = Math.Min(availableDevicesIndex, AvailableDevices.Count - 1);
			if (availableDevicesIndex > -1)
				SelectedAvailableDevice = AvailableDevices[availableDevicesIndex];

			devicesIndex = Math.Min(devicesIndex, Devices.Count - 1);
			if (devicesIndex > -1)
				SelectedDevice = Devices[devicesIndex];

			ServiceFactory.SaveService.GKChanged = true;
		}
		public bool CanAdd(object parameter)
		{
			return SelectedAvailableDevice != null;
		}

		public RelayCommand<object> RemoveCommand { get; private set; }
		public IList SelectedDevices;
		void OnRemove(object parameter)
		{
			var devicesIndex = Devices.IndexOf(SelectedDevice);
			var availableDevicesIndex = AvailableDevices.IndexOf(SelectedAvailableDevice);

			SelectedDevices = (IList)parameter;
			var deviceViewModels = new List<ZoneDeviceViewModel>();
			foreach (var device in SelectedDevices)
			{
				var deviceViewModel = device as ZoneDeviceViewModel;
				if (deviceViewModel != null)
					deviceViewModels.Add(deviceViewModel);
			}
			foreach (var deviceViewModel in deviceViewModels)
			{
				AvailableDevices.Add(deviceViewModel);
				Devices.Remove(deviceViewModel);
				GKManager.RemoveDeviceFromZone(deviceViewModel.Device, Zone);
			}

			SelectedDevice = Devices.FirstOrDefault();
			SelectedAvailableDevice = AvailableDevices.FirstOrDefault();

			availableDevicesIndex = Math.Min(availableDevicesIndex, AvailableDevices.Count - 1);
			if (availableDevicesIndex > -1)
				SelectedAvailableDevice = AvailableDevices[availableDevicesIndex];

			devicesIndex = Math.Min(devicesIndex, Devices.Count - 1);
			if (devicesIndex > -1)
				SelectedDevice = Devices[devicesIndex];

			ServiceFactory.SaveService.GKChanged = true;
		}
		public bool CanRemove(object parameter)
		{
			return SelectedDevice != null && SelectedDevice.Device.Driver.HasZone;
		}
	}
}