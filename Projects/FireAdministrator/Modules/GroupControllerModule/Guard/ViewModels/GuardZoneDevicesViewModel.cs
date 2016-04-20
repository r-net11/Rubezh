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
	public class GuardZoneDevicesViewModel : BaseViewModel
	{
		GKGuardZone Zone;

		public GuardZoneDevicesViewModel()
		{
			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			Devices = new ObservableCollection<GuardZoneDeviceViewModel>();
			AvailableDevices = new ObservableCollection<GuardZoneDeviceViewModel>();
		}

		public void Initialize(GKGuardZone zone)
		{
			Zone = zone;

			Devices = new ObservableCollection<GuardZoneDeviceViewModel>();
			foreach (var device in GKManager.Devices)
			{
				if (device.DriverType == GKDriverType.RSR2_GuardDetector || device.DriverType == GKDriverType.RSR2_GuardDetectorSound || device.DriverType == GKDriverType.RSR2_HandGuardDetector
					|| device.DriverType == GKDriverType.RSR2_AM_1 || device.DriverType == GKDriverType.RSR2_MAP4 || device.Driver.IsCardReaderOrCodeReader)
				{
					var guardZoneDevice = Zone.GuardZoneDevices.FirstOrDefault(x => x.DeviceUID == device.UID);
					if (guardZoneDevice != null)
					{
						var deviceViewModel = new GuardZoneDeviceViewModel(guardZoneDevice);
						Devices.Add(deviceViewModel);
					}
				}
			}
			OnPropertyChanged(() => Devices);
			SelectedDevice = Devices.FirstOrDefault();
		}

		public void Clear()
		{
			Devices.Clear();
			AvailableDevices.Clear();
			SelectedDevice = null;
			SelectedAvailableDevice = null;
		}


		public void InitializeAvailableDevices(GKGuardZone zone)
		{
			AvailableDevices = new ObservableCollection<GuardZoneDeviceViewModel>();
			foreach (var device in GKManager.Devices)
			{
				if (device.Driver.HasGuardZone && !zone.GuardZoneDevices.Any(x => x.DeviceUID == device.UID))
				{
					if (device.GuardZones.Count > 0 || (device.IsInMPT && !GlobalSettingsHelper.GlobalSettings.ShowMPTsDevices)
							|| (device.ZoneUIDs.Count > 0 && !GlobalSettingsHelper.GlobalSettings.ShowZonesDevices)
							|| (device.Door != null && !GlobalSettingsHelper.GlobalSettings.ShowDoorsDevices))
						continue;

					var guardZoneDevice = new GKGuardZoneDevice
					{
						DeviceUID = device.UID,
						Device = device
					};
					var deviceViewModel = new GuardZoneDeviceViewModel(guardZoneDevice);
					AvailableDevices.Add(deviceViewModel);
				}
			}

			OnPropertyChanged(() => AvailableDevices);
			SelectedAvailableDevice = AvailableDevices.FirstOrDefault();
		}
		
		public ObservableCollection<GuardZoneDeviceViewModel> Devices { get; private set; }

		GuardZoneDeviceViewModel _selectedDevice;
		public GuardZoneDeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		public ObservableCollection<GuardZoneDeviceViewModel> AvailableDevices { get; private set; }

		GuardZoneDeviceViewModel _selectedAvailableDevice;
		public GuardZoneDeviceViewModel SelectedAvailableDevice
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
			var availabledeviceViewModels = new List<GuardZoneDeviceViewModel>();
			foreach (var availabledevice in SelectedAvailableDevices)
			{
				var availabledeviceViewModel = availabledevice as GuardZoneDeviceViewModel;
				if (availabledeviceViewModel != null)
					availabledeviceViewModels.Add(availabledeviceViewModel);
			}
			foreach (var availabledeviceViewModel in availabledeviceViewModels)
			{
				Devices.Add(availabledeviceViewModel);
				AvailableDevices.Remove(availabledeviceViewModel);
				GKManager.AddDeviceToGuardZone(Zone, availabledeviceViewModel.GuardZoneDevice);
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
			var deviceViewModels = new List<GuardZoneDeviceViewModel>();
			foreach (var device in SelectedDevices)
			{
				var deviceViewModel = device as GuardZoneDeviceViewModel;
				if (deviceViewModel != null)
				{
					deviceViewModel.Clear();
					deviceViewModels.Add(deviceViewModel);
				}
			}
			foreach (var deviceViewModel in deviceViewModels)
			{
				AvailableDevices.Add(deviceViewModel);
				Devices.Remove(deviceViewModel);
				GKManager.RemoveDeviceFromGuardZone(deviceViewModel.GuardZoneDevice.Device, Zone);
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
			return SelectedDevice != null && SelectedDevice.GuardZoneDevice.Device.Driver.HasGuardZone;
		}
	}
}