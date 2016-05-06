using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Common;
using StrazhAPI;
using StrazhAPI.Enums;
using StrazhAPI.Models;
using StrazhAPI.SKD;
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
		private EventWaitHandle _configurationChangedWaitHandle;

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
			ShowWriteConfigurationInAllControllersCommand = new RelayCommand(OnShowWriteConfigurationInAllControllers);
			ShowControllerLocksPasswordsCommand = new RelayCommand(OnShowControllerLocksPasswords, CanShowControllerLocksPasswords);

#if DEBUG
			Logger.Info("Подписываемся на событие изменения конфигурации");
#endif
			SafeFiresecService.ConfigurationChangedEvent -= SafeFiresecService_ConfigurationChangedEvent;
			SafeFiresecService.ConfigurationChangedEvent += SafeFiresecService_ConfigurationChangedEvent;
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

		public RelayCommand ShowWriteConfigurationInAllControllersCommand { get; private set; }
		void OnShowWriteConfigurationInAllControllers()
		{
			if (ServiceFactory.SaveService.SKDChanged)
			{
				MessageBoxService.ShowWarning("Перед выполнением операции необходимо применить конфигурацию.");
				return;
			}

			var writeConfigurationInAllControllersViewModel = new WriteConfigurationInAllControllersViewModel();
			if (!DialogService.ShowModalWindow(writeConfigurationInAllControllersViewModel))
				return;
			if (writeConfigurationInAllControllersViewModel.IsCards
				&& !MessageBoxService.ShowQuestion("Процесс записи пропусков на все контроллеры может занять несколько минут. В это время приложение будет недоступно.\nПродолжить?"))
				return;
			
			//if (!CheckNeedSave())
			//	return;
			//if (!ValidateConfiguration())
			//	return;

			var thread = new Thread(() =>
			{
#if DEBUG
				Logger.Info("Ожидаем сигнала о возможности продолжить работу треда для записи конфигурации на все контроллеры");
#endif
				if (_configurationChangedWaitHandle != null)
					_configurationChangedWaitHandle.WaitOne();
#if DEBUG
				Logger.Info("Получен сигнал о возможности продолжить работу треда для записи конфигурации на все контроллеры");
#endif
				//Thread.Sleep(TimeSpan.FromSeconds(2));

				var failedDevicesUids = new HashSet<Guid>();
				OperationResult<List<Guid>> result = null;
				
				// 1. Записываем графики доступа
				if (writeConfigurationInAllControllersViewModel.IsTimeSchedules)
				{
#if DEBUG
					Logger.Info("Записываем графики доступа");
#endif
					result = FiresecManager.FiresecService.SKDWriteAllTimeSheduleConfiguration();
					if (result.HasError && result.Result != null)
						result.Result.ForEach(x => failedDevicesUids.Add(x));
				}

				// 2. Записываем пароли
				if (writeConfigurationInAllControllersViewModel.IsLockPasswords && (result == null || !result.IsCanceled))
				{
#if DEBUG
					Logger.Info("Записываем пароли");
#endif
					result = FiresecManager.FiresecService.RewriteControllerLocksPasswordsOnAllControllers();
					if (result.HasError && result.Result != null)
						result.Result.ForEach(x => failedDevicesUids.Add(x));
				}

				// 3. Записываем пропуска
				if (writeConfigurationInAllControllersViewModel.IsCards && (result == null || !result.IsCanceled))
				{
#if DEBUG
					Logger.Info("Записываем пропуска");
#endif
					result = FiresecManager.FiresecService.RewriteCardsOnAllControllers();
					if (result.HasError && result.Result != null)
						result.Result.ForEach(x => failedDevicesUids.Add(x));
				}

				ApplicationService.Invoke(() =>
				{
					if (failedDevicesUids.Count > 0)
					{
						LoadingService.Close();
						var sb = new StringBuilder();
						failedDevicesUids.ForEach(x => sb.AppendLine(SKDManager.Devices.FirstOrDefault(device => device.UID == x).Name));
						MessageBoxService.ShowWarning(String.Format("Операция не выполнена на следующих контроллерах:\n\n{0}", sb));
					}

					var oldHasMissmath = HasMissmath;
					SKDManager.Devices.ForEach(x => ClientSettings.SKDMissmatchSettings.Reset(x.UID));
					foreach (var failedDeviceUID in failedDevicesUids)
					{
						ClientSettings.SKDMissmatchSettings.Set(failedDeviceUID);
					}
					OnPropertyChanged(() => HasMissmath);
					if (HasMissmath != oldHasMissmath)
						ServiceFactory.SaveService.SKDChanged = true;
				});

			});
			thread.Name = "DeviceCommandsViewModel OnShowWriteConfigurationInAllControllers";
			thread.Start();
		}

		public RelayCommand ShowControllerLocksPasswordsCommand { get; private set; }
		private void OnShowControllerLocksPasswords()
		{
			var controllerLocksPasswordsViewModel = new ControllerLocksPasswordsViewModel(SelectedDevice);
			if (DialogService.ShowModalWindow(controllerLocksPasswordsViewModel))
			{
			}
		}
		private bool CanShowControllerLocksPasswords()
		{
			return SelectedDevice.Driver.IsController && SelectedDevice.Driver.DriverType != SKDDriverType.ChinaController_1;
		}

		/// <summary>
		/// Проверяет текущую конфигурацию на наличие ошибок
		/// </summary>
		/// <returns>true - ошибок не обнаружено,
		/// false - есть ошибки</returns>
		private bool ValidateConfiguration()
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

		/// <summary>
		/// Проверяет необходимость сохранения конфигурации перед выполнением операций, зависящих от состояния текущей конфигурации
		/// </summary>
		/// <returns>true - текущая конфигурация уже сохранена (нет изменений),
		/// false - текущая конфигурация еще не сохранена (есть изменения) </returns>
		private bool CheckNeedSave()
		{
			if (ServiceFactory.SaveService.SKDChanged)
			{
				if (MessageBoxService.ShowQuestion("Для выполнения этой операции необходимо применить конфигурацию. Применить сейчас?"))
				{
					_configurationChangedWaitHandle = new AutoResetEvent(false);
					var cancelEventArgs = new CancelEventArgs();
#if DEBUG
					Logger.Info("Сигнализируем о том, что необходимо начать процесс сохранения конфигурации");
#endif
					ServiceFactory.Events.GetEvent<SetNewConfigurationEvent>().Publish(cancelEventArgs); // Сигнализируем модулю Администратор, чтобы он сохранил текущую конфигурацию
					return !cancelEventArgs.Cancel;
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

		private void SafeFiresecService_ConfigurationChangedEvent()
		{
#if DEBUG
			Logger.Info("Получен сигнал об изменении конфигурации");
#endif
			if (_configurationChangedWaitHandle != null)
				_configurationChangedWaitHandle.Set();
		}
	}
}