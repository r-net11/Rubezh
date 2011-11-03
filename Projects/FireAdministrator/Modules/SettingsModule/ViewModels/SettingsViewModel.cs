using System.IO;
using System.Runtime.Serialization;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Win32;

namespace SettingsModule.ViewModels
{
    public class SettingsViewModel : RegionViewModel
    {
        public SettingsViewModel()
        {
            ShowDriversCommand = new RelayCommand(OnShowDrivers);
            SaveConfigurationCommand = new RelayCommand(OnSaveConfiguration);
            LoadConfigurationCommand = new RelayCommand(OnLoadConfiguration);
        }

        public void Initialize()
        {
        }

        public RelayCommand ShowDriversCommand { get; private set; }
        void OnShowDrivers()
        {
            var driversView = new DriversView();
            driversView.ShowDialog();
        }

        public RelayCommand SaveConfigurationCommand { get; private set; }
        void OnSaveConfiguration()
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "firesec2 files|*.fsc2";
            if (saveDialog.ShowDialog().Value)
                SaveToFile(CopyFrom(), saveDialog.FileName);
        }

        public RelayCommand LoadConfigurationCommand { get; private set; }
        void OnLoadConfiguration()
        {
            var openDialog = new OpenFileDialog();
            openDialog.Filter = "firesec2 files|*.fsc2";
            if (openDialog.ShowDialog().Value)
            {
                CopyTo(LoadFromFile(openDialog.FileName));

                FiresecManager.UpdateConfiguration();
                ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
            }
        }

        FullConfiguration CopyFrom()
        {
            return new FullConfiguration()
            {
                DeviceConfiguration = FiresecManager.DeviceConfiguration,
                LibraryConfiguration = FiresecManager.LibraryConfiguration,
                PlansConfiguration = FiresecManager.PlansConfiguration,
                SecurityConfiguration = FiresecManager.SecurityConfiguration,
                SystemConfiguration = FiresecManager.SystemConfiguration
            };
        }

        void CopyTo(FullConfiguration fullConfiguration)
        {
            FiresecManager.DeviceConfiguration = fullConfiguration.DeviceConfiguration;
            FiresecManager.LibraryConfiguration = fullConfiguration.LibraryConfiguration;
            FiresecManager.PlansConfiguration = fullConfiguration.PlansConfiguration;
            FiresecManager.SecurityConfiguration = fullConfiguration.SecurityConfiguration;
            FiresecManager.SystemConfiguration = fullConfiguration.SystemConfiguration;
        }

        FullConfiguration LoadFromFile(string fileName)
        {
            try
            {
                var dataContractSerializer = new DataContractSerializer(typeof(FullConfiguration));
                using (var fileStream = new FileStream(fileName, FileMode.Open))
                {
                    return (FullConfiguration) dataContractSerializer.ReadObject(fileStream);
                }
            }
            catch
            {
                return new FullConfiguration();
            }
        }

        void SaveToFile(FullConfiguration fullConfiguration, string fileName)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(FullConfiguration));
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, fullConfiguration);
            }
        }
    }
}