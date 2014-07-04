using System.ComponentModel;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Microsoft.Win32;

namespace SKDModule.ViewModels
{
	public class DeviceCommandsViewModel : BaseViewModel
	{
		DevicesViewModel DevicesViewModel;

		public DeviceCommandsViewModel(DevicesViewModel devicesViewModel)
		{
			DevicesViewModel = devicesViewModel;
			ShowControllerConfigurationCommand = new RelayCommand(OnShowControllerConfiguration, CanShowControllerConfiguration);
			ShowLockConfigurationCommand = new RelayCommand(OnShowLockConfiguration, CanShowLockConfiguration);
		}

		public DeviceViewModel SelectedDevice
		{
			get { return DevicesViewModel.SelectedDevice; }
		}

		public RelayCommand ShowControllerConfigurationCommand { get; private set; }
		void OnShowControllerConfiguration()
		{
			var controllerPropertiesViewModel = new ControllerPropertiesViewModel(SelectedDevice.Device);
			DialogService.ShowModalWindow(controllerPropertiesViewModel);
		}
		bool CanShowControllerConfiguration()
		{
			return SelectedDevice != null && SelectedDevice.Device.Driver.IsController;
		}

		public RelayCommand ShowLockConfigurationCommand { get; private set; }
		void OnShowLockConfiguration()
		{
			var lockPropertiesViewModel = new LockPropertiesViewModel(SelectedDevice.Device);
			DialogService.ShowModalWindow(lockPropertiesViewModel);
		}
		bool CanShowLockConfiguration()
		{
			return SelectedDevice != null && SelectedDevice.Device.Driver.DriverType == SKDDriverType.Lock;
		}
	}
}