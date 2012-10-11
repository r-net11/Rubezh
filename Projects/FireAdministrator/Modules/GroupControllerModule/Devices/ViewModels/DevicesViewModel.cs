using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using GKModule.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DevicesViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public static DevicesViewModel Current { get; private set; }
		public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }

		public DevicesViewModel()
		{
			Menu = new DevicesMenuViewModel(this);
			Current = this;
			CopyCommand = new RelayCommand(OnCopy, CanCutCopy);
			CutCommand = new RelayCommand(OnCut, CanCutCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			DeviceCommandsViewModel = new DeviceCommandsViewModel(this);
		}

		public void Initialize()
		{
			BuildTree();
			if (Devices.Count > 0)
			{
				CollapseChild(Devices[0]);
				ExpandChild(Devices[0]);
				SelectedDevice = Devices[0];
			}
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
			if (deviceUID != Guid.Empty)
			{
				FillAllDevices();

				var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
				if (deviceViewModel != null)
					deviceViewModel.ExpantToThis();
				SelectedDevice = deviceViewModel;
			}
		}
		#endregion

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

		void BuildTree()
		{
			Devices = new ObservableCollection<DeviceViewModel>();
			AddDevice(XManager.DeviceConfiguration.RootDevice, null);
		}

		public DeviceViewModel AddDevice(XDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device, Devices);
			deviceViewModel.Parent = parentDeviceViewModel;

			var indexOf = Devices.IndexOf(parentDeviceViewModel);
			Devices.Insert(indexOf + 1, deviceViewModel);

			if (device != null)
				foreach (var childDevice in device.Children)
				{
					var childDeviceViewModel = AddDevice(childDevice, deviceViewModel);
					deviceViewModel.Children.Add(childDeviceViewModel);
				}

			return deviceViewModel;
		}

		public void CollapseChild(DeviceViewModel parentDeviceViewModel)
		{
			parentDeviceViewModel.IsExpanded = false;
			foreach (var deviceViewModel in parentDeviceViewModel.Children)
			{
				CollapseChild(deviceViewModel);
			}
		}

		public void ExpandChild(DeviceViewModel parentDeviceViewModel)
		{
			if (parentDeviceViewModel.Device != null && (parentDeviceViewModel.Device.Driver.DriverType == XDriverType.System || parentDeviceViewModel.Device.Driver.DriverType == XDriverType.GK))
			{
				parentDeviceViewModel.IsExpanded = true;
				foreach (var deviceViewModel in parentDeviceViewModel.Children)
				{
					ExpandChild(deviceViewModel);
				}
			}
		}

		XDevice _deviceToCopy;
		bool _isFullCopy;

		bool CanCutCopy()
		{
			return !(SelectedDevice == null || SelectedDevice.Parent == null ||
				SelectedDevice.Driver.IsAutoCreate ||
				(SelectedDevice.Parent.Driver.IsGroupDevice && SelectedDevice.Parent.Driver.GroupDeviceChildType == SelectedDevice.Driver.DriverType));
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			_deviceToCopy = XManager.CopyDevice(SelectedDevice.Device, false);
		}

		public RelayCommand CutCommand { get; private set; }
		void OnCut()
		{
			_deviceToCopy = XManager.CopyDevice(SelectedDevice.Device, true);
			SelectedDevice.RemoveCommand.Execute();

			XManager.DeviceConfiguration.Update();
			ServiceFactory.SaveService.XDevicesChanged = true;
		}

		bool CanPaste()
		{
			return (SelectedDevice != null && _deviceToCopy != null && SelectedDevice.Driver.Children.Contains(_deviceToCopy.Driver.DriverType));
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var pasteDevice = XManager.CopyDevice(_deviceToCopy, _isFullCopy);
			PasteDevice(pasteDevice);
		}

		void PasteDevice(XDevice xDevice)
		{
			SelectedDevice.Device.Children.Add(xDevice);
			xDevice.Parent = SelectedDevice.Device;

			var newDevice = AddDevice(xDevice, SelectedDevice);
			SelectedDevice.Children.Add(newDevice);
			CollapseChild(newDevice);

			XManager.DeviceConfiguration.Update();
			ServiceFactory.SaveService.XDevicesChanged = true;
		}
	}
}