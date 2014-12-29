using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class ZoneDevicesViewModel : BaseViewModel
	{
		Zone Zone;

		public ZoneDevicesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			ShowZoneLogicCommand = new RelayCommand(OnShowZoneLogic, CanShowZoneLogic);
		}

		public void Initialize(Zone zone)
		{
			Zone = zone;

			var devices = new HashSet<Device>();
			var availableDevices = new HashSet<Device>();

			foreach (var device in FiresecManager.Devices)
			{
				if (device.IsNotUsed)
					continue;
				if (zone.ZoneType == ZoneType.Fire && device.Driver.DriverType == DriverType.AM1_O)
					continue;
				if (zone.ZoneType == ZoneType.Guard && device.Driver.DriverType != DriverType.AM1_O)
					continue;

				if (device.Driver.IsZoneDevice)
				{
					if (device.ZoneUID == Guid.Empty)
					{
						device.AllParents.ForEach(x => { availableDevices.Add(x); });
						availableDevices.Add(device);
					}

					if (device.ZoneUID != Guid.Empty && device.ZoneUID == Zone.UID)
					{
						device.AllParents.ForEach(x => { devices.Add(x); });
						devices.Add(device);
					}
				}

				if (device.Driver.IsZoneLogicDevice && device.ZoneLogic != null && device.ZoneLogic.Clauses.IsNotNullOrEmpty())
				{
					foreach (var clause in device.ZoneLogic.Clauses.Where(x => x.ZoneUIDs.Contains(Zone.UID)))
					{
						device.AllParents.ForEach(x => { devices.Add(x); });
						devices.Add(device);
					}
				}
			}

			deviceViewModels = new List<DeviceViewModel>();
			foreach (var device in devices)
			{
				var deviceViewModel = new DeviceViewModel(device, false)
				{
					IsExpanded = true,
					IsBold = device.Driver.IsZoneDevice || device.Driver.IsZoneLogicDevice
				};
				deviceViewModels.Add(deviceViewModel);
			}

			foreach (var device in deviceViewModels.Where(x => x.Device.Parent != null))
			{
				var parent = deviceViewModels.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
				parent.AddChild(device);
			}

			availableDeviceViewModels = new List<DeviceViewModel>();
			foreach (var device in availableDevices)
			{
				var deviceViewModel = new DeviceViewModel(device, false)
				{
					IsExpanded = true,
					IsBold = device.Driver.IsZoneDevice
				};
				availableDeviceViewModels.Add(deviceViewModel);
			}

			foreach (var device in availableDeviceViewModels.Where(x => x.Device.Parent != null))
			{
				var parent = availableDeviceViewModels.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
				parent.AddChild(device);
			}

			RootDevice = deviceViewModels.FirstOrDefault(x => x.Parent == null);
			AvailableRootDevice = availableDeviceViewModels.FirstOrDefault(x => x.Parent == null);

			OnPropertyChanged(() => RootDevices);
			OnPropertyChanged(() => AvailableRootDevices);
		}

		public void Clear()
		{
			deviceViewModels.Clear();
			availableDeviceViewModels.Clear();
			RootDevice = null;
			AvailableRootDevice = null;
			OnPropertyChanged(() => RootDevices);
			OnPropertyChanged(() => AvailableRootDevices);
			SelectedDevice = null;
			SelectedAvailableDevice = null;
		}

		List<DeviceViewModel> deviceViewModels = new List<DeviceViewModel>();
		List<DeviceViewModel> availableDeviceViewModels = new List<DeviceViewModel>();

		public DeviceViewModel RootDevice{get;private set;}
		public ObservableCollection<DeviceViewModel> RootDevices
		{
			get
			{
				if (RootDevice == null)
					return new ObservableCollection<DeviceViewModel>();
				return new ObservableCollection<DeviceViewModel>() { RootDevice };
			}
		}

		public DeviceViewModel AvailableRootDevice { get; private set; }
		public ObservableCollection<DeviceViewModel> AvailableRootDevices
		{
			get
			{
				if (AvailableRootDevice == null)
					return new ObservableCollection<DeviceViewModel>();
				return new ObservableCollection<DeviceViewModel>() { AvailableRootDevice };
			}
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

		DeviceViewModel _selectedAvailableDevice;
		public DeviceViewModel SelectedAvailableDevice
		{
			get { return _selectedAvailableDevice; }
			set
			{
				_selectedAvailableDevice = value;
				OnPropertyChanged(() => SelectedAvailableDevice);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var oldIndex = availableDeviceViewModels.IndexOf(SelectedAvailableDevice);
			var oldDeviceUID = SelectedAvailableDevice.Device.UID;

			FiresecManager.FiresecConfiguration.AddDeviceToZone(SelectedAvailableDevice.Device, Zone);
			Initialize(Zone);

			SelectedDevice = deviceViewModels.FirstOrDefault(x => x.Device.UID == oldDeviceUID);
			if (availableDeviceViewModels.Count > 0)
			{
				var newIndex = System.Math.Min(oldIndex, availableDeviceViewModels.Count - 1);
				SelectedAvailableDevice = availableDeviceViewModels[newIndex];
			}
			else
			{
				SelectedAvailableDevice = null;
			}

			ServiceFactory.SaveService.FSChanged = true;
		}
		public bool CanAdd()
		{
			return SelectedAvailableDevice != null && SelectedAvailableDevice.IsZoneDevice;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var oldIndex = deviceViewModels.IndexOf(SelectedDevice);
			var oldDeviceUID = SelectedDevice.Device.UID;

			FiresecManager.FiresecConfiguration.RemoveDeviceFromZone(SelectedDevice.Device, Zone);
			Initialize(Zone);

			SelectedAvailableDevice = availableDeviceViewModels.FirstOrDefault(x => x.Device.UID == oldDeviceUID);
			if (deviceViewModels.Count > 0)
			{
				var newIndex = System.Math.Min(oldIndex, deviceViewModels.Count - 1);
				SelectedDevice = deviceViewModels[newIndex];
			}
			else
			{
				SelectedDevice = null;
			}

			ServiceFactory.SaveService.FSChanged = true;
		}
		public bool CanRemove()
		{
			return SelectedDevice != null && SelectedDevice.IsZoneDevice;
		}

		public RelayCommand ShowZoneLogicCommand { get; private set; }
		void OnShowZoneLogic()
		{
			var zoneLogicViewModel = new ZoneLogicViewModel(SelectedDevice.Device);
			if (DialogService.ShowModalWindow(zoneLogicViewModel))
			{
				ServiceFactory.SaveService.FSChanged = true;
				Initialize(Zone);
			}
		}
		public bool CanShowZoneLogic()
		{
			return SelectedDevice != null && SelectedDevice.Device.Driver.IsZoneLogicDevice;
		}
	}
}