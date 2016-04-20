using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2;

namespace AdministratorTestClientFS2.ViewModels
{
	public class DevicesViewModel : DialogViewModel
	{
		private DeviceViewModel _rootDevice;
		private DeviceViewModel _selectedDevice;
		public static DevicesViewModel Current { get; private set; }

		public DevicesViewModel()
		{
			Current = this;
			BuildTree();
			OnPropertyChanged("RootDevices");
		}

		public DevicesViewModel(Device device)
		{
			BuildTree(device);
			if (RootDevice != null)
			{
				RootDevice.IsExpanded = true;
				SelectedDevice = RootDevice;
			}
			OnPropertyChanged("RootDevices");
		}

		#region DeviceSelection
		public List<DeviceViewModel> AllDevices;

		public void FillAllDevices()
		{
			AllDevices = new List<DeviceViewModel>();
			AddChildPlainDevices(RootDevice);
		}

		void AddChildPlainDevices(DeviceViewModel parentViewModel)
		{
			AllDevices.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainDevices(childViewModel);
		}

		public static void UpdateGuardVisibility()
		{
			var hasSecurityDevice = FiresecManager.Devices.Any(x => x.Driver.DeviceType == DeviceType.Sequrity);
			//ServiceFactory.Events.GetEvent<GuardVisibilityChangedEvent>().Publish(hasSecurityDevice);
		}

		public void Select(Guid deviceUID)
		{
			if (deviceUID != Guid.Empty)
			{
				FillAllDevices();
				var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
				if (deviceViewModel != null)
					deviceViewModel.ExpandToThis();
				SelectedDevice = deviceViewModel;
			}
		}
		#endregion

		public DeviceViewModel AddDevice(Device device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = AddDeviceInternal(device, parentDeviceViewModel);
			FillAllDevices();
			return deviceViewModel;
		}

		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged("SelectedDevice");
			}
		}

		public DeviceViewModel RootDevice
		{
			get { return _rootDevice; }
			private set
			{
				_rootDevice = value;
				OnPropertyChanged("RootDevice");
			}
		}

		public DeviceViewModel[] RootDevices
		{
			get { return new[] { RootDevice }; }
		}

		private void BuildTree(Device device)
		{
			RootDevice = AddDeviceInternal(device, null);
			FillAllDevices();
		}

		private void BuildTree()
		{
			RootDevice = AddDeviceInternal(ConfigurationManager.DeviceConfiguration.RootDevice, null);
			FillAllDevices();
		}

		static DeviceViewModel AddDeviceInternal(Device device, TreeItemViewModel<DeviceViewModel> parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.Children.Add(deviceViewModel);

			foreach (Device childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);
			return deviceViewModel;
		}
	}
}