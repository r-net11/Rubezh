using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using Common;
using FireAdministrator.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Microsoft.Win32;
using FiresecAPI;

namespace FireAdministrator
{
    public static class FileConfigurationHelper
    {
        public static void SaveToFile()
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

        public static void LoadFromFile()
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

                    ServiceFactory.SaveService.DevicesChanged = true;
                    ServiceFactory.SaveService.PlansChanged = true;
                    ServiceFactory.Layout.ShowFooter(null);
                });
            }
        }

        static FullConfiguration CopyFrom()
        {
            return new FullConfiguration()
            {
                DeviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration,
                //DeviceLibraryConfiguration = FiresecManager.DeviceLibraryConfiguration,
                PlansConfiguration = FiresecManager.PlansConfiguration,
                SecurityConfiguration = FiresecManager.SecurityConfiguration,
                SystemConfiguration = FiresecManager.SystemConfiguration,
                XDeviceConfiguration = XManager.DeviceConfiguration,
                Version = new ConfigurationVersion() { MajorVersion = 1, MinorVersion = 1 }
            };
        }

        static void CopyTo(FullConfiguration fullConfiguration)
        {
            FiresecManager.FiresecConfiguration.DeviceConfiguration = fullConfiguration.DeviceConfiguration;
            if (FiresecManager.FiresecConfiguration.DeviceConfiguration == null)
                FiresecManager.FiresecConfiguration.SetEmptyConfiguration();
            //FiresecManager.DeviceLibraryConfiguration = fullConfiguration.DeviceLibraryConfiguration;
            FiresecManager.PlansConfiguration = fullConfiguration.PlansConfiguration;
            FiresecManager.SecurityConfiguration = fullConfiguration.SecurityConfiguration;
            FiresecManager.SystemConfiguration = fullConfiguration.SystemConfiguration;
            XManager.DeviceConfiguration = fullConfiguration.XDeviceConfiguration;
            if (XManager.DeviceConfiguration == null)
                XManager.SetEmptyConfiguration();
        }

        static FullConfiguration LoadFromFile(string fileName)
        {
            try
            {
                var dataContractSerializer = new DataContractSerializer(typeof(FullConfiguration));
                using (var fileStream = new FileStream(fileName, FileMode.Open))
                {
                    FullConfiguration fullConfiguration = (FullConfiguration)dataContractSerializer.ReadObject(fileStream);
                    fullConfiguration.ValidateVersion();
                    return fullConfiguration;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове MenuView.LoadFromFile");
                return new FullConfiguration();
            }
        }

        static void SaveToFile(FullConfiguration fullConfiguration, string fileName)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(FullConfiguration));
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, fullConfiguration);
            }
        }
    }
}