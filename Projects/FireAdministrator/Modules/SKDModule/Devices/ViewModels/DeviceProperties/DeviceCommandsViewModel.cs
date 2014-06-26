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

			WriteConfigCommand = new RelayCommand(OnWriteConfig, CanWriteConfig);
			ShowInfoCommand = new RelayCommand(OnShowInfo, CanShowInfo);
			SynchroniseTimeCommand = new RelayCommand(OnSynchroniseTime, CanSynchroniseTime);
			ShowPasswordCommand = new RelayCommand(OnShowPassword, CanShowPassword);
			WriteAllIdentifiersCommand = new RelayCommand(OnWriteAllIdentifiers, CanWriteAllIdentifiers);
		}

		public DeviceViewModel SelectedDevice
		{
			get { return DevicesViewModel.SelectedDevice; }
		}

		public RelayCommand ShowInfoCommand { get; private set; }
		void OnShowInfo()
		{
			var result = FiresecManager.FiresecService.SKDGetDeviceInfo(SelectedDevice.Device);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error, "Ошибка во время операции");
			}
			else
			{
				var text = "SoftwareBuildDate = " + result.Result.SoftwareBuildDate.ToString() + "\n" +
					"DeviceType = " + result.Result.DeviceType + "\n" +
					"SoftwareVersion = " + result.Result.SoftwareVersion + "\n" +
					"IP = " + result.Result.IP + "\n" +
					"SubnetMask = " + result.Result.SubnetMask + "\n" +
					"DefaultGateway = " + result.Result.DefaultGateway + "\n" +
					"MTU = " + result.Result.MTU + "\n" +
					"DateTime = " + result.Result.CurrentDateTime.ToString();
				MessageBoxService.Show(text);
			}
		}
		bool CanShowInfo()
		{
			return SelectedDevice != null && SelectedDevice.Device.Driver.IsController;
		}

		public RelayCommand SynchroniseTimeCommand { get; private set; }
		void OnSynchroniseTime()
		{
			var result = FiresecManager.FiresecService.SKDSyncronyseTime(SelectedDevice.Device);
			if (result.Result)
			{
				MessageBoxService.Show("Операция синхронизации времени завершилась успешно");
			}
			else
			{
				MessageBoxService.ShowWarning(result.Error, "Ошибка во время операции синхронизации времени");
			}
		}
		bool CanSynchroniseTime()
		{
			return SelectedDevice != null && SelectedDevice.Device.Driver.IsController;
		}

		public RelayCommand ShowPasswordCommand { get; private set; }
		void OnShowPassword()
		{
			var passwordViewModel = new PasswordViewModel(SelectedDevice.Device);
			DialogService.ShowModalWindow(passwordViewModel);
		}
		bool CanShowPassword()
		{
			return SelectedDevice != null && SelectedDevice.Device.Driver.IsController;
		}

		public RelayCommand WriteConfigCommand { get; private set; }
		void OnWriteConfig()
		{
			if (CheckNeedSave(true))
			{
				if (ValidateConfiguration())
				{
					var result = FiresecManager.FiresecService.SKDWriteConfiguration(SelectedDevice.Device);
					if (result.HasError)
					{
						MessageBoxService.ShowError(result.Error);
					}
				}
			}
		}

		bool CanWriteConfig()
		{
			return FiresecManager.CheckPermission(PermissionType.Adm_WriteDeviceConfig) &&
				SelectedDevice != null && SelectedDevice.Device.DriverType == SKDDriverType.Controller;
		}

		public RelayCommand WriteAllIdentifiersCommand { get; private set; }
		void OnWriteAllIdentifiers()
		{
			if (SelectedDevice.Device.DriverType == SKDDriverType.Controller)
			{
				var result = FiresecManager.FiresecService.SKDWriteAllIdentifiers(SelectedDevice.Device);
				if (result.HasError)
					MessageBoxService.ShowError(result.Error, "Ошибка при обновление ПО");
			}
		}
		bool CanWriteAllIdentifiers()
		{
			return SelectedDevice != null && SelectedDevice.Driver.DriverType == SKDDriverType.Controller;
		}

		bool ValidateConfiguration()
		{
			var validationResult = ServiceFactory.ValidationService.Validate();
			if (validationResult.HasErrors("SKD"))
			{
				if (validationResult.CannotSave("SKD") || validationResult.CannotWrite("SKD"))
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
				if (MessageBoxService.ShowQuestion("Для выполнения этой операции необходимо применить конфигурацию. Применить сейчас?") == System.Windows.MessageBoxResult.Yes)
				{
					//if (syncParameters)
					//	SelectedDevice.SyncFromSystemToDeviceProperties(SelectedDevice.GetRealChildren());
					var cancelEventArgs = new CancelEventArgs();
					ServiceFactory.Events.GetEvent<SetNewConfigurationEvent>().Publish(cancelEventArgs);
					return !cancelEventArgs.Cancel;
				}
				return false;
			}
			return true;
		}
	}
}