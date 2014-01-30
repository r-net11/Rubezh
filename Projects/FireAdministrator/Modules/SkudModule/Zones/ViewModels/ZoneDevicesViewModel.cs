using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class ZoneDevicesViewModel : BaseViewModel
	{
		SKDZone Zone;

		public ZoneDevicesViewModel()
		{
			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			Devices = new ObservableCollection<ZoneDeviceViewModel>();
			AvailableDevices = new ObservableCollection<ZoneDeviceViewModel>();
		}

		public void Initialize(SKDZone zone)
		{
			Zone = zone;

			var devices = new HashSet<SKDDevice>();
			var availableDevices = new HashSet<SKDDevice>();

			if (!zone.IsRootZone)
			{
				foreach (var device in SKDManager.Devices)
				{
					if (device.Driver.HasZone)
					{
						if (device.ZoneUID == Zone.UID)
						{
							devices.Add(device);
						}
						else if (device.ZoneUID == Guid.Empty)
						{
							availableDevices.Add(device);
						}
					}
				}
			}

			Devices = new ObservableCollection<ZoneDeviceViewModel>();
			foreach (var device in devices)
			{
				var deviceViewModel = new ZoneDeviceViewModel(device)
				{
					IsBold = device.ZoneUID == Zone.UID
				};
				Devices.Add(deviceViewModel);
			}

			var selectedDevice = Devices.LastOrDefault();
			AvailableDevices = new ObservableCollection<ZoneDeviceViewModel>();
			foreach (var device in availableDevices)
			{
				if (device.DriverType == SKDDriverType.System || device.DriverType == SKDDriverType.System)
					continue;

				var deviceViewModel = new ZoneDeviceViewModel(device)
				{
					IsBold = device.Driver.HasZone
				};

				AvailableDevices.Add(deviceViewModel);
			}

			var selectedAvailableDevice = AvailableDevices.LastOrDefault();
			//AvailableDevices = new ObservableCollection<ZoneDeviceViewModel>(AvailableDevices.Where(x => x.Device.Parent == null));
			OnPropertyChanged(() => Devices);
			OnPropertyChanged(() => AvailableDevices);

			SelectedDevice = selectedDevice;
			SelectedAvailableDevice = selectedAvailableDevice;
		}

		public void Clear()
		{
			Devices.Clear();
			AvailableDevices.Clear();
			SelectedDevice = null;
			SelectedAvailableDevice = null;
		}

		public void UpdateAvailableDevices()
		{
			OnPropertyChanged("AvailableDevices");
		}

		public ObservableCollection<ZoneDeviceViewModel> Devices { get; private set; }

		ZoneDeviceViewModel _selectedDevice;
		public ZoneDeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");
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
				OnPropertyChanged("SelectedAvailableDevice");
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
				SKDManager.AddDeviceToZone(availabledeviceViewModel.Device, Zone);
			}

			Initialize(Zone);

			availableDevicesIndex = Math.Min(availableDevicesIndex, AvailableDevices.Count - 1);
			if (availableDevicesIndex > -1)
				SelectedAvailableDevice = AvailableDevices[availableDevicesIndex];

			devicesIndex = Math.Min(devicesIndex, Devices.Count - 1);
			if (devicesIndex > -1)
				SelectedDevice = Devices[devicesIndex];

			ServiceFactory.SaveService.SKDChanged = true;
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
				SKDManager.RemoveDeviceFromZone(deviceViewModel.Device, Zone);
			}

			Initialize(Zone);

			availableDevicesIndex = Math.Min(availableDevicesIndex, AvailableDevices.Count - 1);
			if (availableDevicesIndex > -1)
				SelectedAvailableDevice = AvailableDevices[availableDevicesIndex];

			devicesIndex = Math.Min(devicesIndex, Devices.Count - 1);
			if (devicesIndex > -1)
				SelectedDevice = Devices[devicesIndex];

			ServiceFactory.SaveService.SKDChanged = true;
		}
		public bool CanRemove(object parameter)
		{
			return SelectedDevice != null && SelectedDevice.Device.Driver.HasZone;
		}
	}
}