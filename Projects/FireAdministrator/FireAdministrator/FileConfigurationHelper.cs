using System;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Win32;

namespace FireAdministrator
{
    public static class FileConfigurationHelper
    {
        public static void SaveToFile()
        {
            try
            {
                var saveDialog = new SaveFileDialog()
                {
                    Filter = "firesec2 files|*.fsc2",
                    DefaultExt = "firesec2 files|*.fsc2"
                };
                if (saveDialog.ShowDialog().Value)
                {
                    WaitHelper.Execute(() =>
                    {
                        SaveToFile(CopyFrom(), saveDialog.FileName);
                    });
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "MenuView.SaveToFile");
                MessageBox.Show(e.Message, "Ошибка при выполнении операции");
            }
        }

        public static void LoadFromFile()
        {
            try
            {
                var openDialog = new OpenFileDialog()
                {
                    Filter = "firesec2 files|*.fsc2",
                    DefaultExt = "firesec2 files|*.fsc2"
                };
                if (openDialog.ShowDialog().Value)
                {
                    WaitHelper.Execute(() =>
                    {
                        CopyTo(LoadFromFile(openDialog.FileName));

                        FiresecManager.UpdateConfiguration();
                        XManager.UpdateConfiguration();
                        ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);

                        ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
                        ServiceFactory.Layout.Close();
                        ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);

                        ServiceFactory.SaveService.FSChanged = true;
                        ServiceFactory.SaveService.PlansChanged = true;
                        ServiceFactory.SaveService.GKChanged = true;
                        ServiceFactory.Layout.ShowFooter(null);
                    });
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "MenuView.LoadFromFile");
                MessageBox.Show(e.Message, "Ошибка при выполнении операции");
            }
        }

        static FullConfiguration CopyFrom()
        {
            try
            {
                return new FullConfiguration()
                {
                    DeviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration,
                    PlansConfiguration = FiresecManager.PlansConfiguration,
                    SystemConfiguration = FiresecManager.SystemConfiguration,
                    XDeviceConfiguration = XManager.DeviceConfiguration,
                    Version = new ConfigurationVersion() { MajorVersion = 1, MinorVersion = 1 }
                };
            }
            catch (Exception e)
            {
                Logger.Error(e, "MenuView.CopyFrom");
                throw e;
            }
        }

        static void CopyTo(FullConfiguration fullConfiguration)
        {
            try
            {
                FiresecManager.FiresecConfiguration.DeviceConfiguration = fullConfiguration.DeviceConfiguration;
                if (FiresecManager.FiresecConfiguration.DeviceConfiguration == null)
                    FiresecManager.FiresecConfiguration.SetEmptyConfiguration();
                FiresecManager.PlansConfiguration = fullConfiguration.PlansConfiguration;
                FiresecManager.SystemConfiguration = fullConfiguration.SystemConfiguration;
                XManager.DeviceConfiguration = fullConfiguration.XDeviceConfiguration;
                if (XManager.DeviceConfiguration == null)
                    XManager.SetEmptyConfiguration();
            }
            catch (Exception e)
            {
                Logger.Error(e, "MenuView.CopyTo");
                throw e;
            }
        }

        static FullConfiguration LoadFromFile(string fileName)
        {
            try
            {
                FullConfiguration fullConfiguration = null;
                var dataContractSerializer = new DataContractSerializer(typeof(FullConfiguration));
                using (var fileStream = new FileStream(fileName, FileMode.Open))
                {
                    fullConfiguration = (FullConfiguration)dataContractSerializer.ReadObject(fileStream);
                }
                if (!fullConfiguration.ValidateVersion())
                    SaveToFile(fullConfiguration, fileName);
                return fullConfiguration;
            }
            catch (Exception e)
            {
                Logger.Error(e, "MenuView.LoadFromFile");
                throw e;
            }
        }

        static void SaveToFile(FullConfiguration fullConfiguration, string fileName)
        {
            try
            {
                var dataContractSerializer = new DataContractSerializer(typeof(FullConfiguration));
                using (var fileStream = new FileStream(fileName, FileMode.Create))
                {
                    dataContractSerializer.WriteObject(fileStream, fullConfiguration);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "MenuView.SaveToFile");
                throw e;
            }
        }
    }
}