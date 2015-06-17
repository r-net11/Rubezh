﻿using System.ComponentModel;
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
using System.Threading;
using System;
using System.Linq;
using Infrastructure.Common.Services;

namespace StrazhModule.ViewModels
{
	public class DeviceCommandsViewModel : BaseViewModel
	{
		DevicesViewModel DevicesViewModel;

		public DeviceCommandsViewModel(DevicesViewModel devicesViewModel)
		{
			DevicesViewModel = devicesViewModel;
			ShowControllerConfigurationCommand = new RelayCommand(OnShowControllerConfiguration, CanShowController);
			ShowControllerDoorTypeCommand = new RelayCommand(OnShowControllerDoorType, CanShowControllerDoorType);
			ShowControllerPasswordCommand = new RelayCommand(OnShowControllerPassword, CanShowController);
			ShowControllerNetworkCommand = new RelayCommand(OnShowControllerNetwork, CanShowController);
			ShowControllerTimeSettingsCommand = new RelayCommand(OnShowControllerTimeSettings, CanShowController);
			ShowLockConfigurationCommand = new RelayCommand(OnShowLockConfiguration, CanShowLockConfiguration);
			WriteTimeSheduleConfigurationCommand = new RelayCommand(OnWriteTimeSheduleConfiguration, CanShowController);
			WriteAllTimeSheduleConfigurationCommand = new RelayCommand(OnWriteAllTimeSheduleConfiguration);
		}

		public DeviceViewModel SelectedDevice
		{
			get { return DevicesViewModel.SelectedDevice; }
		}

		public RelayCommand ShowControllerConfigurationCommand { get; private set; }
		void OnShowControllerConfiguration()
		{
			var deviceInfoResult = FiresecManager.FiresecService.SKDGetDeviceInfo(SelectedDevice.Device);
			if (!deviceInfoResult.HasError)
			{
				var controllerPropertiesViewModel = new ControllerPropertiesViewModel(SelectedDevice.Device, deviceInfoResult.Result);
				DialogService.ShowModalWindow(controllerPropertiesViewModel);
			}
			else
			{
				MessageBoxService.ShowWarning(deviceInfoResult.Error);
			}
		}
		bool CanShowController()
		{
			return SelectedDevice != null && SelectedDevice.Device.Driver.IsController;
		}

		public RelayCommand ShowControllerDoorTypeCommand { get; private set; }
		void OnShowControllerDoorType()
		{
			var previousDoorType = SelectedDevice.Device.DoorType;

			var controllerDoorTypeViewModel = new ControllerDoorTypeViewModel(SelectedDevice);
			if (DialogService.ShowModalWindow(controllerDoorTypeViewModel))
			{
				if (previousDoorType != SelectedDevice.Device.DoorType)
				{
					foreach (var childDevice in SelectedDevice.Device.Children)
					{
						if (childDevice.Door != null)
						{
							var doorViewModel = DoorsViewModel.Current.Doors.FirstOrDefault(x => x.Door.UID == childDevice.Door.UID);
							SKDManager.ChangeDoorDevice(childDevice.Door, null);
							if (doorViewModel != null)
							{
								doorViewModel.Update(doorViewModel.Door);
							}
						}
					}
				}

				foreach (var deviceViewModel in SelectedDevice.Children)
				{
					deviceViewModel.Update();
				}
			}
		}

		bool CanShowControllerDoorType()
		{
			return SelectedDevice != null && SelectedDevice.Driver.IsController;
		}

		public RelayCommand ShowControllerPasswordCommand { get; private set; }
		void OnShowControllerPassword()
		{
			var controllerPaswordViewModel = new ControllerPaswordViewModel(SelectedDevice);
			if (DialogService.ShowModalWindow(controllerPaswordViewModel))
			{

			}
		}

		public RelayCommand ShowControllerNetworkCommand { get; private set; }
		void OnShowControllerNetwork()
		{
			var controllerNetSettingsViewModel = new ControllerNetSettingsViewModel(SelectedDevice);
			if (DialogService.ShowModalWindow(controllerNetSettingsViewModel))
			{

			}
		}

		public RelayCommand ShowControllerTimeSettingsCommand { get; private set; }
		void OnShowControllerTimeSettings()
		{
			var controllerTimeSettingsViewModel = new ControllerTimeSettingsViewModel(SelectedDevice);
			if (DialogService.ShowModalWindow(controllerTimeSettingsViewModel))
			{

			}
		}

		public RelayCommand ShowLockConfigurationCommand { get; private set; }
		void OnShowLockConfiguration()
		{
			var lockPropertiesViewModel = new LockPropertiesViewModel(SelectedDevice.Device);
			if (DialogService.ShowModalWindow(lockPropertiesViewModel))
			{
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanShowLockConfiguration()
		{
			return SelectedDevice != null && SelectedDevice.Device.Driver.DriverType == SKDDriverType.Lock && SelectedDevice.Device.IsEnabled;
		}

		public RelayCommand WriteTimeSheduleConfigurationCommand { get; private set; }
		void OnWriteTimeSheduleConfiguration()
		{
			if (CheckNeedSave(true))
			{
				if (ValidateConfiguration())
				{
					var thread = new Thread(() =>
					{
						var result = FiresecManager.FiresecService.SKDWriteTimeSheduleConfiguration(SelectedDevice.Device);

						ApplicationService.Invoke(new Action(() =>
						{
							if (result.HasError)
							{
								LoadingService.Close();
								MessageBoxService.ShowWarning(result.Error);
							}

							var oldHasMissmath = HasMissmath;
							if (ClientSettings.SKDMissmatchSettings.HasMissmatch(SelectedDevice.Device.UID))
							{
								if (result.HasError)
								{
									ClientSettings.SKDMissmatchSettings.Set(SelectedDevice.Device.UID);
								}
								else
								{
									ClientSettings.SKDMissmatchSettings.Reset(SelectedDevice.Device.UID);
								}
							}
							OnPropertyChanged(() => HasMissmath);
							if (HasMissmath != oldHasMissmath)
								ServiceFactory.SaveService.SKDChanged = true;
						}));
					});
					thread.Name = "DeviceCommandsViewModel OnWriteTimeSheduleConfiguration";
					thread.Start();
				}
			}
		}

		public RelayCommand WriteAllTimeSheduleConfigurationCommand { get; private set; }
		void OnWriteAllTimeSheduleConfiguration()
		{
			if (CheckNeedSave(true))
			{
				if (ValidateConfiguration())
				{
					var thread = new Thread(() =>
					{
						var result = FiresecManager.FiresecService.SKDWriteAllTimeSheduleConfiguration();

						ApplicationService.Invoke(new Action(() =>
						{
							if (result.HasError)
							{
								LoadingService.Close();
								MessageBoxService.ShowWarning(result.Error);
							}

							var oldHasMissmath = HasMissmath;
							SKDManager.Devices.ForEach(x => ClientSettings.SKDMissmatchSettings.Reset(x.UID));
							foreach (var failedDeviceUID in result.Result)
							{
								ClientSettings.SKDMissmatchSettings.Set(failedDeviceUID);
							}
							OnPropertyChanged(() => HasMissmath);
							if (HasMissmath != oldHasMissmath)
								ServiceFactory.SaveService.SKDChanged = true;
						}));
					});
					thread.Name = "DeviceCommandsViewModel OnWriteTimeSheduleConfiguration";
					thread.Start();
				}
			}
		}

		bool ValidateConfiguration()
		{
			var validationResult = ServiceFactory.ValidationService.Validate();
			if (validationResult.HasErrors(ModuleType.SKD))
			{
				if (validationResult.CannotSave(ModuleType.SKD) || validationResult.CannotWrite(ModuleType.SKD))
				{
					MessageBoxService.ShowWarning("Обнаружены ошибки. Операция прервана");
					return false;
				}
			}
			return true;
		}

		bool CheckNeedSave(bool syncParameters = false)
		{
			if (ServiceFactory.SaveService.SKDChanged)
			{
				if (MessageBoxService.ShowQuestion("Для выполнения этой операции необходимо применить конфигурацию. Применить сейчас?"))
				{
					var cancelEventArgs = new CancelEventArgs();
					ServiceFactory.Events.GetEvent<SetNewConfigurationEvent>().Publish(cancelEventArgs);
					//return !cancelEventArgs.Cancel;
				}
				return false;
			}
			return true;
		}

		public bool HasMissmath
		{
			get
			{
				return false;

				foreach (var device in SKDManager.Devices)
				{
					if (device.Driver.IsController)
					{
						if (ClientSettings.SKDMissmatchSettings.HasMissmatch(device.UID))
							return true;
					}
				}
				return false;
			}
		}
	}
}