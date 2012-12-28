using System;
using System.Windows;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using System.IO;

namespace FireAdministrator
{
    public static class ConfigManager
    {
        public static bool SetNewConfig()
        {
            try
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

					ConfigFileHelper.SaveToFile();
					var fileStream = new FileStream("TempConfig.fscp", FileMode.Open);
					FiresecManager.FiresecService.SetConfig(fileStream);

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

                    if (ServiceFactory.SaveService.XLibraryChanged)
                    {
                        LoadingService.DoStep("Сохранение конфигурации библиотеки устройств ГК");
                        FiresecManager.FiresecService.SetXDeviceLibraryConfiguration(XManager.XDeviceLibraryConfiguration);
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
            catch (Exception e)
            {
                Logger.Error(e, "MenuView.SetNewConfig");
				MessageBoxService.ShowError(e.Message, "Ошибка при выполнении операции");
                return false;
            }
        }

        public static void CreateNew()
        {
            try
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
            catch (Exception e)
            {
                Logger.Error(e, "MenuView.CreateNew");
                MessageBox.Show(e.Message, "Ошибка при выполнении операции");
            }
        }
    }
}