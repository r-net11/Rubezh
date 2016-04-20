using System.Linq;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.GK;
using Infrastructure.Common.Windows;
using Infrastructure;
using RubezhClient;
using Infrastructure.Events;
using Infrastructure.Common.Windows.Services;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class AutoSearchDevicesViewModel : DialogViewModel
	{
		GKDevice LocalDevice;
		GKDevice RemoteDevice;

		public AutoSearchDevicesViewModel(GKDevice remoteDevice, GKDevice localDevice)
		{
			Title = "Устройства, найденные в результате автопоиска";
			ChangeCommand = new RelayCommand(OnChange, CanChange);

			RemoteDevice = remoteDevice;
			LocalDevice = localDevice;
			new GKDeviceConfiguration { RootDevice = RemoteDevice }.UpdateConfiguration();
			RootDevice = AddDeviceInternal(RemoteDevice, null);
			if (SelectedDevice != null)
				SelectedDevice.ExpandToThis();
		}
		DeviceViewModel AddDeviceInternal(GKDevice device, DeviceViewModel parentDeviceViewModel)
		{
			if (device.DriverType == GKDriverType.RSR2_MVP)
				foreach (var autoCreateDriverType in device.Driver.AutoCreateChildren)
				{
					var autoCreateDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == autoCreateDriverType);
					for (byte i = autoCreateDriver.MinAddress; i <= autoCreateDriver.MaxAddress; i++)
					{
						GKManager.AddDevice(device, autoCreateDriver, i);
					}
				}
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
			if (LocalDevice.DriverType == GKDriverType.GK)
			{
				LocalDevice.Children.RemoveAll(x => x.DriverType == GKDriverType.RSR2_KAU || x.DriverType == GKDriverType.GKMirror);
				foreach (var kauChild in RemoteDevice.Children)
				{
					if (kauChild.DriverType == GKDriverType.RSR2_KAU || kauChild.DriverType == GKDriverType.GKMirror)
					{
						LocalDevice.Children.Add(kauChild);
					}
				}
			}
			if (LocalDevice.DriverType == GKDriverType.RSR2_KAU)
			{
				LocalDevice.Children.RemoveAll(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif);
				foreach (var alsChild in RemoteDevice.Children)
				{
					if (alsChild.DriverType == GKDriverType.RSR2_KAU_Shleif)
					{
						LocalDevice.Children.Add(alsChild);
					}
				}
			}
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