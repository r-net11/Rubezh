using System;
using System.IO;
using System.Linq;
using System.Windows;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Ionic.Zip;

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
							if (FiresecManager.FiresecDriver != null)
							{
								var fsResult = FiresecManager.FiresecDriver.SetNewConfig(FiresecManager.FiresecConfiguration.DeviceConfiguration);
								if (fsResult.HasError)
								{
									MessageBoxService.ShowError(fsResult.Error);
								}
							}
						}
					}

					var tempFileName = Path.GetTempFileName() + "_";
					var zipFile = new ZipFile(tempFileName);

					TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

					if (ServiceFactory.SaveService.FSChanged)
					{
						AddConfiguration(zipFile, FiresecManager.FiresecConfiguration.DeviceConfiguration, "DeviceConfiguration.xml", 1, 1);
					}
					if (ServiceFactory.SaveService.PlansChanged)
					{
						AddConfiguration(zipFile, FiresecManager.PlansConfiguration, "PlansConfiguration.xml", 1, 1);
					}
					if (ServiceFactory.SaveService.SecurityChanged)
					{
						AddConfiguration(zipFile, FiresecManager.SecurityConfiguration, "SecurityConfiguration.xml", 1, 1);
					}
					if (ServiceFactory.SaveService.LibraryChanged)
					{
						AddConfiguration(zipFile, FiresecManager.DeviceLibraryConfiguration, "DeviceLibraryConfiguration.xml", 1, 1);
					}
					if ((ServiceFactory.SaveService.InstructionsChanged) ||
						(ServiceFactory.SaveService.SoundsChanged) ||
						(ServiceFactory.SaveService.FilterChanged) ||
						(ServiceFactory.SaveService.CamerasChanged))
					{
						AddConfiguration(zipFile, FiresecManager.SystemConfiguration, "SystemConfiguration.xml", 1, 1);
					}
					if (ServiceFactory.SaveService.GKChanged)
					{
						AddConfiguration(zipFile, XManager.DeviceConfiguration, "XDeviceConfiguration.xml", 1, 1);
					}
					if (ServiceFactory.SaveService.XLibraryChanged)
					{
						AddConfiguration(zipFile, XManager.XDeviceLibraryConfiguration, "XDeviceLibraryConfiguration.xml", 1, 1);
					}

					AddConfiguration(zipFile, TempZipConfigurationItemsCollection, "ZipConfigurationItemsCollection.xml", 1, 1);
					zipFile.Save(tempFileName);
					zipFile.Dispose();

					using (var fileStream = new FileStream(tempFileName, FileMode.Open))
					{
						FiresecManager.FiresecService.SetConfig(fileStream);
					}
					File.Delete(tempFileName);

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

		static ZipConfigurationItemsCollection TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

		static void AddConfiguration(ZipFile zipFile, VersionedConfiguration configuration, string name, int minorVersion, int majorVersion)
		{
			configuration.BeforeSave();
			configuration.Version = new ConfigurationVersion() { MinorVersion = minorVersion, MajorVersion = majorVersion };
			var configurationStream = ZipSerializeHelper.Serialize(configuration);
			if (zipFile.Entries.Any(x => x.FileName == name))
				zipFile.RemoveEntry(name);
			configurationStream.Position = 0;
			zipFile.AddEntry(name, configurationStream);

			TempZipConfigurationItemsCollection.ZipConfigurationItems.Add(new ZipConfigurationItem(name, minorVersion, majorVersion));
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