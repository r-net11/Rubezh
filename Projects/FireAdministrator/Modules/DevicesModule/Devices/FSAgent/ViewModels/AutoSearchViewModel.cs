using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class AutoSearchViewModel : SaveCancelDialogViewModel
	{
		public List<AutoSearchDeviceViewModel> Devices { get; set; }
		List<AutoSearchDeviceViewModel> allDevices;

		public AutoSearchViewModel(DeviceConfiguration autodetectedDeviceConfiguration)
		{
			Title = "Добавление устройств";
			ContinueCommand = new RelayCommand(OnContinue);

			allDevices = new List<AutoSearchDeviceViewModel>();
			Devices = new List<AutoSearchDeviceViewModel>();
			Devices.Add(AddDevice(autodetectedDeviceConfiguration.RootDevice, null));
		}

		AutoSearchDeviceViewModel AddDevice(Device device, AutoSearchDeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new AutoSearchDeviceViewModel(device);

			foreach (var childDevice in device.Children)
			{
				if (childDevice.Driver == null)
					continue;

				var childDeviceViewModel = AddDevice(childDevice, deviceViewModel);
				deviceViewModel.Children.Add(childDeviceViewModel);
			}

			allDevices.Add(deviceViewModel);
			return deviceViewModel;
		}

		public RelayCommand ContinueCommand { get; private set; }
		void OnContinue()
		{
			Close(true);
		}

		public bool CanContinue
		{
			//get { return !FiresecManager.IsFS2Enabled; }
			get { return true; }
		}

		void AddFromTree(AutoSearchDeviceViewModel parentAutoDetectedDevice)
		{
			foreach (var autodetectedDevice in parentAutoDetectedDevice.Children)
			{
				if (autodetectedDevice.IsSelected)
				{
					AddAutoDevice(autodetectedDevice.Device);
					AddFromTree(autodetectedDevice);
				}
			}
		}

		void AddAutoDevice(Device device)
		{
			var parentDevice = FiresecManager.Devices.FirstOrDefault(x => x.PathId == device.Parent.PathId);
			if (parentDevice != null)
			{
				if (!parentDevice.Children.Any(x => x.UID == device.UID))
				{
					parentDevice.Children.Add(device);
					FiresecManager.FiresecConfiguration.DeviceConfiguration.Update();
				}
			}

			var parentDeviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device.PathId == device.Parent.PathId);
			if (parentDeviceViewModel != null)
			{
				var deviceViewModel = new DeviceViewModel(device);
				parentDeviceViewModel.AddChild(deviceViewModel);
				DevicesViewModel.Current.AllDevices.Add(deviceViewModel);
				parentDeviceViewModel.Update();
			}
		}

		protected override bool Save()
		{
			try
			{
				FiresecManager.FiresecConfiguration.DeviceConfiguration.UpdateIdPath();
				AddFromTree(Devices[0]);
			}
			catch (Exception e)
			{
				MessageBoxService.ShowException(e);
				Logger.Error(e, "AutoSearchViewModel.Save");
			}
			ServiceFactory.SaveService.FSChanged = true;
			Close(false);
			return false;
		}
	}
}