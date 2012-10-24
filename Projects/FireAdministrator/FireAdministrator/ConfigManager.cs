using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows;
using System.Windows;
using FiresecClient;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Events;
using Infrastructure.Common;

namespace FireAdministrator
{
	public static class ConfigManager
	{
		public static bool SetNewConfig()
		{
			ServiceFactory.Events.GetEvent<ConfigurationSavingEvent>().Publish(null);

			var validationResult = ServiceFactory.ValidationService.Validate();
			if (validationResult.HasErrors())
			{
				if (validationResult.CannotSave())
				{
					MessageBoxService.ShowWarning("Обнаружены ошибки. Операция прервана");
					return false;
				}

				if (MessageBoxService.ShowQuestion("Конфигурация содержит ошибки. Продолжить?") != MessageBoxResult.Yes)
					return false;
			}

			WaitHelper.Execute(() =>
			{
				LoadingService.ShowProgress("Применение конфигурации", "Применение конфигурации", 10);
				if (ServiceFactory.SaveService.FSChanged)
				{
					LoadingService.DoStep("Применение конфигурации устройств");
					if (!ServiceFactory.AppSettings.DoNotOverrideFS1)
					{
						var fsResult = FiresecManager.FiresecDriver.SetNewConfig(FiresecManager.FiresecConfiguration.DeviceConfiguration);
						if (fsResult.HasError)
						{
							MessageBoxService.ShowError(fsResult.Error);
						}
					}
					LoadingService.DoStep("Сохранение конфигурации устройств");
					var result = FiresecManager.FiresecService.SetDeviceConfiguration(FiresecManager.FiresecConfiguration.DeviceConfiguration);
					if (result.HasError)
					{
						MessageBoxService.ShowError(result.Error);
					}
				}

				if (ServiceFactory.SaveService.PlansChanged)
				{
					LoadingService.DoStep("Сохранение конфигурации графических планов");
					FiresecManager.FiresecService.SetPlansConfiguration(FiresecManager.PlansConfiguration);
				}

				if (ServiceFactory.SaveService.SecurityChanged)
				{
					LoadingService.DoStep("Сохранение конфигурации пользователей и ролей");
					FiresecManager.FiresecService.SetSecurityConfiguration(FiresecManager.SecurityConfiguration);
				}

				if (ServiceFactory.SaveService.LibraryChanged)
				{
					LoadingService.DoStep("Сохранение конфигурации библиотеки устройств");
					FiresecManager.FiresecService.SetDeviceLibraryConfiguration(FiresecManager.DeviceLibraryConfiguration);
				}

				if ((ServiceFactory.SaveService.InstructionsChanged) ||
					(ServiceFactory.SaveService.SoundsChanged) ||
					(ServiceFactory.SaveService.FilterChanged) ||
					(ServiceFactory.SaveService.CamerasChanged))
				{
					LoadingService.DoStep("Сохранение конфигурации прочих настроек");
					FiresecManager.FiresecService.SetSystemConfiguration(FiresecManager.SystemConfiguration);
				}

				if (ServiceFactory.SaveService.GKChanged)
				{
					LoadingService.DoStep("Сохранение конфигурации ГК");
					FiresecManager.FiresecService.SetXDeviceConfiguration(XManager.DeviceConfiguration);
				}

				if (ServiceFactory.SaveService.FSChanged ||
					ServiceFactory.SaveService.PlansChanged ||
					ServiceFactory.SaveService.GKChanged)
					FiresecManager.FiresecService.NotifyClientsOnConfigurationChanged();
			});
			LoadingService.Close();
			ServiceFactory.SaveService.Reset();
			return true;
		}

		public static void CreateNew()
		{
			var result = MessageBoxService.ShowQuestion("Вы уверены, что хотите создать новую конфигурацию");
			if (result == MessageBoxResult.Yes)
			{
				FiresecManager.SetEmptyConfiguration();
				XManager.SetEmptyConfiguration();
				FiresecManager.PlansConfiguration = new PlansConfiguration();
				FiresecManager.SystemConfiguration = new SystemConfiguration();
				FiresecManager.PlansConfiguration.Update();

				ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);

				ServiceFactory.SaveService.FSChanged = true;
				ServiceFactory.SaveService.PlansChanged = true;
				ServiceFactory.SaveService.InstructionsChanged = true;
				ServiceFactory.SaveService.SoundsChanged = true;
				ServiceFactory.SaveService.FilterChanged = true;
				ServiceFactory.SaveService.CamerasChanged = true;
				ServiceFactory.SaveService.GKChanged = true;

				ServiceFactory.Layout.Close();
				ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);
				ServiceFactory.Layout.ShowFooter(null);
			}
		}
	}
}