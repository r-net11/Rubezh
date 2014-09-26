﻿using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class DeviceSelectionViewModel : SaveCancelDialogViewModel
	{
		public DeviceSelectionViewModel(XDevice device)
		{
			Title = "Выбор устройства";
			RootDevice = AddDeviceInternal(XManager.DeviceConfiguration.RootDevice, null);
			RootDevice.GetAllChildren().FindAll(x => x.Device.DriverType == XDriverType.KAU_Shleif || x.Device.DriverType == XDriverType.RSR2_KAU_Shleif).ForEach(y => y.ExpandToThis());
			if (device != null)
				SelectedDevice = RootDevice.GetAllChildren().FirstOrDefault(x => x.Device.UID == device.UID);
			if (SelectedDevice == null)
				SelectedDevice = RootDevice.GetAllChildren().FirstOrDefault();
			if(SelectedDevice != null)
				SelectedDevice.ExpandToThis();
		}

		DeviceViewModel AddDeviceInternal(XDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);
			return deviceViewModel;
		}

		DeviceViewModel _rootDevice;
		public DeviceViewModel RootDevice
		{
			get { return _rootDevice; }
			private set
			{
				_rootDevice = value;
				OnPropertyChanged(() => RootDevice);
				OnPropertyChanged(() => RootDevices);
			}
		}
		public DeviceViewModel[] RootDevices
		{
			get { return new DeviceViewModel[] { RootDevice }; }
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

		protected override bool CanSave()
		{
			return ((SelectedDevice == null) || ((SelectedDevice.Device.Driver.IsControlDevice) || (SelectedDevice.Device.Driver.IsDeviceOnShleif)));
		}
	}
}