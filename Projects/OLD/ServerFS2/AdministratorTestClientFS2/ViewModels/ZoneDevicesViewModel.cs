using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace AdministratorTestClientFS2.ViewModels
{
	public class ZoneDevicesViewModel : BaseViewModel
	{
		Zone Zone;

		public ZoneDevicesViewModel()
		{
		}

		public void Initialize(Zone zone)
		{
			Zone = zone;

			var devices = new HashSet<Device>();
			var availableDevices = new HashSet<Device>();

			foreach (var device in FiresecManager.Devices)
			{
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
				parent.Children.Add(device);
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
				parent.Children.Add(device);
			}

			RootDevice = deviceViewModels.FirstOrDefault(x => x.Parent == null);
			AvailableRootDevice = availableDeviceViewModels.FirstOrDefault(x => x.Parent == null);

			OnPropertyChanged("RootDevices");
			OnPropertyChanged("AvailableRootDevices");
		}

		public void Clear()
		{
			deviceViewModels.Clear();
			availableDeviceViewModels.Clear();
			RootDevice = null;
			AvailableRootDevice = null;
			OnPropertyChanged("RootDevices");
			OnPropertyChanged("AvailableRootDevices");
			SelectedDevice = null;
			SelectedAvailableDevice = null;
		}

		List<DeviceViewModel> deviceViewModels = new List<DeviceViewModel>();
		List<DeviceViewModel> availableDeviceViewModels = new List<DeviceViewModel>();

		public DeviceViewModel RootDevice { get; private set; }
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
				OnPropertyChanged("SelectedDevice");
			}
		}

		DeviceViewModel _selectedAvailableDevice;
		public DeviceViewModel SelectedAvailableDevice
		{
			get { return _selectedAvailableDevice; }
			set
			{
				_selectedAvailableDevice = value;
				OnPropertyChanged("SelectedAvailableDevice");
			}
		}
	}
}