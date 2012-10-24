using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class DeviceConfigurationViewModel : DialogViewModel
	{
		Guid _deviceUID;
		DeviceConfiguration _deviceConfiguration;
		Device LocalRootDevice;
		Device RemoteRootDevice;

		public DeviceConfigurationViewModel(Guid deviceUID, DeviceConfiguration deviceConfiguration)
		{
			Title = "Сравнение конфигураций";
			ReplaceCommand = new RelayCommand(OnReplace);
			_deviceUID = deviceUID;
			deviceConfiguration.Reorder();
			_deviceConfiguration = deviceConfiguration;
			_deviceConfiguration.Update();
			foreach (var device in _deviceConfiguration.Devices)
			{
				device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
			}

			LocalRootDevice = FiresecManager.Devices.FirstOrDefault(x => x.UID == _deviceUID);
			RemoteRootDevice = _deviceConfiguration.Devices.FirstOrDefault(x => x.UID == _deviceUID);
			var UnionRootDevice = (Device)FiresecManager.Devices.FirstOrDefault(x => x.UID == _deviceUID).Clone();
			UnionRootDevice.Children = new List<Device>();
			foreach (var localChild in LocalRootDevice.Children)
			{
				localChild.IsLocal = true;
				localChild.IsRemote = false;
				UnionRootDevice.Children.Add(localChild);
			}

			foreach (var remoteChild in RemoteRootDevice.Children)
			{
				remoteChild.IsRemote = true;
				remoteChild.IsLocal = false;
				var localAndRemote = LocalRootDevice.Children.FirstOrDefault(x => x.PresentationAddressAndDriver == remoteChild.PresentationAddressAndDriver);
				if (localAndRemote != null)
				{
					UnionRootDevice.Children.FirstOrDefault(
						x => x.PresentationAddressAndDriver == localAndRemote.PresentationAddressAndDriver).IsRemote = true;
				}
				else
				{
					UnionRootDevice.Children.Add(remoteChild);
				}
			}

			LocalDevices = new DeviceTreeViewModel(UnionRootDevice, FiresecManager.FiresecConfiguration.DeviceConfiguration);
			RemoteDevices = new DeviceTreeViewModel(UnionRootDevice, _deviceConfiguration);
		}

		public DeviceTreeViewModel LocalDevices { get; private set; }
		public DeviceTreeViewModel RemoteDevices { get; private set; }

		public RelayCommand ReplaceCommand { get; private set; }
		void OnReplace()
		{
			if (MessageBoxService.ShowQuestion("Обратите внимание, что при наличии межпанельных связей, информация о внешних устройствах может быть восстановлена не полностью") != System.Windows.MessageBoxResult.Yes)
				return;

			var parent = LocalRootDevice.Parent;
			parent.Children.Remove(LocalRootDevice);
			parent.Children.Add(RemoteRootDevice);
			RemoteRootDevice.Parent = parent;
			//LocalRootDevice = RemoteRootDevice;

			var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device.UID == RemoteRootDevice.UID);
			deviceViewModel.CollapseChildren();
			deviceViewModel.Children.Clear();
			foreach (var device in RemoteRootDevice.Children)
				DevicesViewModel.Current.AddDevice(device, deviceViewModel);
			deviceViewModel.ExpandChildren();

			FiresecManager.FiresecConfiguration.DeviceConfiguration.Update();
			ServiceFactory.SaveService.FSChanged = true;
			DevicesViewModel.UpdateGuardVisibility();

			Close(true);
		}
	}
}