using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using Infrastructure.Common;
using Infrastructure;
using FiresecClient;
using Infrastructure.Events;
using Infrastructure.Common.Services;

namespace GKModule.ViewModels
{
	public class AutoSearchDevicesViewModel : DialogViewModel
	{
		GKDevice LocalDevice;
		GKDeviceConfiguration RemoteDeviceConfiguration;

		public AutoSearchDevicesViewModel(GKDeviceConfiguration deviceConfiguration, GKDevice localDevice)
		{
			Title = "Устройства, найденные в результате автопоиска";
			ChangeCommand = new RelayCommand(OnChange, CanChange);
			RemoteDeviceConfiguration = deviceConfiguration;
			LocalDevice = localDevice;

			deviceConfiguration.UpdateConfiguration();
			RootDevice = AddDeviceInternal(deviceConfiguration.RootDevice, null);
			if (SelectedDevice != null)
				SelectedDevice.ExpandToThis();
		}
		DeviceViewModel AddDeviceInternal(GKDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);

			if (deviceViewModel.Device.DriverType == GKDriverType.RSR2_KAU_Shleif)
				deviceViewModel.ExpandToThis();

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

		public RelayCommand ChangeCommand { get; private set; }
		void OnChange()
		{
			var RemoteDevice = RemoteDeviceConfiguration.Devices.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
			var LocalConfiguration = GKManager.DeviceConfiguration;

			LocalDevice.Children.RemoveAll(x => x.DriverType == GKDriverType.RSR2_KAU);
			foreach (var kauChild in RemoteDevice.Children)
			{
				if (kauChild.DriverType == GKDriverType.RSR2_KAU)
				{
					LocalDevice.Children.Add(kauChild);
				}
			}

			LocalConfiguration.Zones.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
			LocalConfiguration.Directions.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
			LocalConfiguration.PumpStations.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
			LocalConfiguration.MPTs.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
			LocalConfiguration.Delays.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
			LocalConfiguration.GuardZones.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
			LocalConfiguration.Codes.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
			LocalConfiguration.Doors.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);

			ServiceFactory.SaveService.GKChanged = true;
			GKManager.UpdateConfiguration();
			ServiceFactoryBase.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
			Close(true);
		}

		bool CanChange()
		{
			return true;
		}
	}
}