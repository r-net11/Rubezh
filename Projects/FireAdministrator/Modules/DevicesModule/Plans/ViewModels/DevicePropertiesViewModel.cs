using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

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

			foreach (var device in FiresecManager.DeviceConfiguration.Devices)
			{
				var deviceViewModel = new DeviceViewModel(device, Devices);
				deviceViewModel.IsExpanded = true;
				Devices.Add(deviceViewModel);
			}

			foreach (var device in Devices)
			{
				if (device.Device.Parent != null)
				{
					var parent = Devices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
					device.Parent = parent;
					parent.Children.Add(device);
				}
			}

			if (Devices.Count > 0)
			{
				CollapseChild(Devices[0]);
				ExpandChild(Devices[0]);
			}

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
			foreach (var childViewModel in parentViewModel.Children)
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
				deviceViewModel.ExpantToThis();
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
				OnPropertyChanged("SelectedDevice");
			}
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
			if (parentDeviceViewModel.Device.Driver.Category != DeviceCategoryType.Device)
			{
				parentDeviceViewModel.IsExpanded = true;
				foreach (var deviceViewModel in parentDeviceViewModel.Children)
				{
					ExpandChild(deviceViewModel);
				}
			}
		}

		protected override bool Save()
		{
			Guid deviceUID = _elementDevice.DeviceUID;
			//Helper.SetDevice(_elementDevice, SelectedDevice.Device);

			if (deviceUID != _elementDevice.DeviceUID)
				Update(deviceUID);
			Update(_elementDevice.DeviceUID);
			return base.Save();
		}
		private void Update(Guid deviceUID)
		{
			var device = _devicesViewModel.Devices.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (device != null)
				device.Update();
		}
	}
}
