using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DevicesViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public void Initialize()
		{
			StatesWatcher.RequestAllStates();

			BuildDeviceTree();
			if (Devices.Count > 0)
			{
				CollapseChild(Devices[0]);
				ExpandChild(Devices[0]);
				SelectedDevice = Devices[0];
			}
		}

		ObservableCollection<DeviceViewModel> _devices;
		public ObservableCollection<DeviceViewModel> Devices
		{
			get { return _devices; }
			set
			{
				_devices = value;
				OnPropertyChanged("Devices");
			}
		}

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				if (value != null)
					value.ExpantToThis();
				OnPropertyChanged("SelectedDevice");
			}
		}

		void BuildDeviceTree()
		{
			Devices = new ObservableCollection<DeviceViewModel>();
			AllDevices = new List<DeviceViewModel>();
			AddDevice(XManager.DeviceConfiguration.RootDevice, null);
		}

		DeviceViewModel AddDevice(XDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device, Devices) { Parent = parentDeviceViewModel };
			AllDevices.Add(deviceViewModel);

			Devices.Insert(Devices.IndexOf(parentDeviceViewModel) + 1, deviceViewModel);
			foreach (var childDevice in device.Children)
			{
				if (childDevice.IsNotUsed)
					continue;

				var deviceState = childDevice.DeviceState;
				deviceViewModel.Children.Add(AddDevice(childDevice, deviceViewModel));
			}

			return deviceViewModel;
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
			if ((parentDeviceViewModel.Device.Driver.DriverType == XDriverType.System) ||
				(parentDeviceViewModel.Device.Driver.DriverType == XDriverType.GK) ||
				(parentDeviceViewModel.Device.Driver.DriverType == XDriverType.KAU))
			{
				parentDeviceViewModel.IsExpanded = true;
				foreach (var deviceViewModel in parentDeviceViewModel.Children)
				{
					ExpandChild(deviceViewModel);
				}
			}
		}

		#region DeviceSelection
		public List<DeviceViewModel> AllDevices;

		public void Select(Guid deviceUID)
		{
			if (deviceUID != Guid.Empty)
			{
				var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
				if (deviceViewModel != null)
				{
					deviceViewModel.ExpantToThis();
					SelectedDevice = deviceViewModel;
				}
			}
		}
		#endregion

		public override void OnShow()
		{
			base.OnShow();
		}
	}
}