using System.IO;
using System.Runtime.Serialization;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
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
            {
                var fullConfiguration = CopyFrom();
                SaveToFile(fullConfiguration, saveDialog.FileName);
            }
        }

        public RelayCommand LoadConfigurationCommand { get; private set; }
        void OnLoadConfiguration()
        {
            var openDialog = new OpenFileDialog();
            openDialog.Filter = "firesec2 files|*.fsc2";
            if (openDialog.ShowDialog().Value)
            {
                var fullConfiguration = LoadFromFile(openDialog.FileName);
                CopyTo(fullConfiguration);
            }
        }

        FullConfiguration CopyFrom()
        {
            var fullConfiguration = new FullConfiguration();
            fullConfiguration.DeviceConfiguration = FiresecManager.DeviceConfiguration;
            fullConfiguration.LibraryConfiguration = FiresecManager.LibraryConfiguration;
            fullConfiguration.PlansConfiguration = FiresecManager.PlansConfiguration;
            fullConfiguration.SecurityConfiguration = FiresecManager.SecurityConfiguration;
            fullConfiguration.SystemConfiguration = FiresecManager.SystemConfiguration;
            return fullConfiguration;
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
            FullConfiguration fullConfiguration;
            try
            {
                var dataContractSerializer = new DataContractSerializer(typeof(FullConfiguration));
                using (var fileStream = new FileStream(fileName, FileMode.Open))
                {
                    fullConfiguration = (FullConfiguration)dataContractSerializer.ReadObject(fileStream);
                }

                return fullConfiguration;
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
