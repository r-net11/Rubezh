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
			Devices = new ObservableCollection<DeviceViewModel>();
			AvailableDevices = new ObservableCollection<DeviceViewModel>();
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

			Devices.Clear();
			foreach (var device in devices)
			{
				var deviceViewModel = new DeviceViewModel(device)
				{
					IsExpanded = true,
					IsBold = device.Driver.IsZoneDevice || device.Driver.IsZoneLogicDevice
				};
				Devices.Add(deviceViewModel);
			}

			foreach (var device in Devices.Where(x => x.Device.Parent != null))
			{
				var parent = Devices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
				parent.Children.Add(device);
			}

			AvailableDevices.Clear();
			foreach (var device in availableDevices)
			{
				var deviceViewModel = new DeviceViewModel(device)
				{
					IsExpanded = true,
					IsBold = device.Driver.IsZoneDevice
				};
				AvailableDevices.Add(deviceViewModel);
			}

			foreach (var device in AvailableDevices.Where(x => x.Device.Parent != null))
			{
				var parent = AvailableDevices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
				parent.Children.Add(device);
			}

			OnPropertyChanged("Devices");

			SelectedDevice = Devices.LastOrDefault();
			SelectedAvailableDevice = AvailableDevices.LastOrDefault();
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

		public ObservableCollection<DeviceViewModel> AvailableDevices { get; private set; }

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

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var oldIndex = AvailableDevices.IndexOf(SelectedAvailableDevice);
			var oldDeviceUID = SelectedAvailableDevice.Device.UID;

            FiresecManager.FiresecConfiguration.AddDeviceToZone(SelectedAvailableDevice.Device, Zone);
            Initialize(Zone);
			UpdateAvailableDevices();

			SelectedDevice = Devices.FirstOrDefault(x => x.Device.UID == oldDeviceUID);
			if (AvailableDevices.Count > 0)
			{
				var newIndex = System.Math.Min(oldIndex, AvailableDevices.Count - 1);
				SelectedAvailableDevice = AvailableDevices[newIndex];
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
			var oldIndex = Devices.IndexOf(SelectedDevice);
			var oldDeviceUID = SelectedDevice.Device.UID;

            FiresecManager.FiresecConfiguration.RemoveDeviceFromZone(SelectedDevice.Device, Zone);
            Initialize(Zone);
			UpdateAvailableDevices();

			SelectedAvailableDevice = AvailableDevices.FirstOrDefault(x => x.Device.UID == oldDeviceUID);
			if (Devices.Count > 0)
			{
				var newIndex = System.Math.Min(oldIndex, Devices.Count - 1);
				SelectedDevice = Devices[newIndex];
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