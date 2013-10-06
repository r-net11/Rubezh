using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ZoneDevicesViewModel : BaseViewModel
	{
		XZone Zone;

		public ZoneDevicesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			Devices = new ObservableCollection<ZoneDeviceViewModel>();
			AvailableDevices = new ObservableCollection<ZoneDeviceViewModel>();
		}

		public void Initialize(XZone zone)
		{
			Zone = zone;

			var devices = new HashSet<XDevice>();
			var availableDevices = new HashSet<XDevice>();

			foreach (var device in XManager.Devices)
			{
				if (!device.Driver.HasZone)
					continue;

				if (device.ZoneUIDs.Contains(Zone.UID))
				{
					//device.AllParents.ForEach(x => { devices.Add(x); });
					devices.Add(device);
				}
				else
				{
					if (device.ZoneUIDs.Count == 0)
					{
						//device.AllParents.ForEach(x => { availableDevices.Add(x); });
						availableDevices.Add(device);
					}
				}
			}

			Devices = new ObservableCollection<ZoneDeviceViewModel>();
			foreach (var device in devices)
			{
				var deviceViewModel = new ZoneDeviceViewModel(device)
				{
					IsExpanded = true,
					IsBold = device.ZoneUIDs.Contains(Zone.UID)
				};
				Devices.Add(deviceViewModel);
			}

			//foreach (var device in Devices.Where(x => x.Device.Parent != null))
			//{
			//    var parent = Devices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
			//    if (parent != null)
			//        parent.AddChild(device);
			//}
			var selectedDevice = Devices.LastOrDefault();
			//Devices = new ObservableCollection<ZoneDeviceViewModel>(Devices.Where(x => x.Device.Parent == null));

			AvailableDevices = new ObservableCollection<ZoneDeviceViewModel>();
			foreach (var device in availableDevices)
			{
				if ((device.Driver.DriverType == XDriverType.GKIndicator) ||
					(device.Driver.DriverType == XDriverType.GKLine) ||
					(device.Driver.DriverType == XDriverType.GKRele) ||
					(device.Driver.DriverType == XDriverType.KAUIndicator))
					continue;

				var deviceViewModel = new ZoneDeviceViewModel(device)
				{
					IsExpanded = true,
					IsBold = device.Driver.HasZone
				};

				AvailableDevices.Add(deviceViewModel);
			}

			//foreach (var device in AvailableDevices.Where(x => x.Device.Parent != null))
			//{
			//    var parent = AvailableDevices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
			//    if (parent != null)
			//        parent.AddChild(device);
			//}
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

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var oldIndex = AvailableDevices.IndexOf(SelectedAvailableDevice);
			var newIndex = 0;
			var newDeviceUID = Guid.Empty;
			for (int currentIndex = 0; currentIndex < AvailableDevices.Count; currentIndex++)
			{
				var deviceViewModel = AvailableDevices[currentIndex];
				if (!deviceViewModel.IsBold)
					continue;
				if (deviceViewModel == SelectedAvailableDevice)
					continue;
				if (Math.Abs(currentIndex - oldIndex) <= Math.Abs(oldIndex - newIndex))
				{
					newIndex = currentIndex;
					newDeviceUID = deviceViewModel.Device.UID;
				}
			}

			XManager.AddDeviceToZone(SelectedAvailableDevice.Device, Zone);
			Initialize(Zone);
			UpdateAvailableDevices();
			SelectedAvailableDevice = AvailableDevices.FirstOrDefault(x => x.Device.UID == newDeviceUID);
			ServiceFactory.SaveService.GKChanged = true;
		}
		public bool CanAdd()
		{
			return ((SelectedAvailableDevice != null) && (SelectedAvailableDevice.Driver.HasZone));
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var oldIndex = Devices.IndexOf(SelectedDevice);
			var newIndex = 0;
			var newDeviceUID = Guid.Empty;
			for (int currentIndex = 0; currentIndex < Devices.Count; currentIndex++)
			{
				var deviceViewModel = Devices[currentIndex];
				if (!deviceViewModel.IsBold)
					continue;
				if (deviceViewModel == SelectedDevice)
					continue;
				if (Math.Abs(currentIndex - oldIndex) <= Math.Abs(oldIndex - newIndex))
				{
					newIndex = currentIndex;
					newDeviceUID = deviceViewModel.Device.UID;
				}
			}

			XManager.RemoveDeviceFromZone(SelectedDevice.Device, Zone);
			Initialize(Zone);
			UpdateAvailableDevices();
			SelectedDevice = Devices.FirstOrDefault(x => x.Device.UID == newDeviceUID);
			ServiceFactory.SaveService.GKChanged = true;
		}
		public bool CanRemove()
		{
			return ((SelectedDevice != null) && (SelectedDevice.Driver.HasZone));
		}
	}
}