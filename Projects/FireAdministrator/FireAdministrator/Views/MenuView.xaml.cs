using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using Controls.MessageBox;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;
using Microsoft.Win32;
using XFiresecAPI;

namespace FireAdministrator.Views
{
    public partial class MenuView : UserControl
    {
        public MenuView()
        {
            InitializeComponent();
            DataContext = this;
            ServiceFactory.SaveService.Changed += new Action(SaveService_Changed);
        }

        void SaveService_Changed()
        {
            _saveButton.IsEnabled = true;
        }

        void OnSetNewConfig(object sender, RoutedEventArgs e)
        {
            if (MessageBoxService.ShowQuestion("Вы уверены, что хотите перезаписать текущую конфигурацию?") == MessageBoxResult.Yes)
            {
                SetNewConfig();
            }
        }

        public void SetNewConfig()
        {
            if (CanChangeConfig == false)
            {
                MessageBoxService.Show("У вас нет прав на сохранение конфигурации");
                return;
            }

            PlansModule.PlansModule.Save();

            var validationErrorsViewModel = new ValidationErrorsViewModel();
            if (validationErrorsViewModel.HasErrors)
            {
                ServiceFactory.Layout.ShowValidationArea(new ValidationErrorsViewModel());

                if (validationErrorsViewModel.CannotSave)
                {
                    MessageBoxService.ShowWarning("Обнаружены ошибки. Операция прервана");
                    return;
                }

                if (MessageBoxService.ShowQuestion("Конфигурация содержит ошибки. Продолжить?") != MessageBoxResult.Yes)
                    return;
            }

            if (ServiceFactory.SaveService.DevicesChanged)
                FiresecManager.FiresecService.SetDeviceConfiguration(FiresecManager.DeviceConfiguration);

            if (ServiceFactory.SaveService.PlansChanged)
                FiresecManager.FiresecService.SetPlansConfiguration(FiresecManager.PlansConfiguration);

            if (ServiceFactory.SaveService.SecurityChanged)
                FiresecManager.FiresecService.SetSecurityConfiguration(FiresecManager.SecurityConfiguration);

            if (ServiceFactory.SaveService.LibraryChanged)
                FiresecManager.FiresecService.SetLibraryConfiguration(FiresecManager.LibraryConfiguration);

            if ((ServiceFactory.SaveService.InstructionsChanged) ||
                (ServiceFactory.SaveService.SoundsChanged) ||
                (ServiceFactory.SaveService.FilterChanged) ||
                (ServiceFactory.SaveService.CamerasChanged))
                FiresecManager.FiresecService.SetSystemConfiguration(FiresecManager.SystemConfiguration);

            if (ServiceFactory.SaveService.XDevicesChanged)
                FiresecManager.FiresecService.SetXDeviceConfiguration(XManager.DeviceConfiguration);

            ServiceFactory.SaveService.Reset();
            _saveButton.IsEnabled = false;
        }

        void OnCreateNew(object sender, RoutedEventArgs e)
        {
            var result = MessageBoxService.ShowQuestion("Вы уверены, что хотите создать новую конфигурацию");
            if (result == MessageBoxResult.Yes)
            {
                FiresecManager.DeviceConfiguration = new DeviceConfiguration();
                FiresecManager.LibraryConfiguration = new LibraryConfiguration();
                FiresecManager.PlansConfiguration = new PlansConfiguration();
                FiresecManager.SystemConfiguration = new SystemConfiguration();

                FiresecManager.DeviceConfiguration.RootDevice = new Device()
                {
                    DriverUID = FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Computer).UID,
                    Driver = FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Computer)
                };
                FiresecManager.DeviceConfiguration.Update();
                FiresecManager.PlansConfiguration.Update();

                XManager.DeviceConfiguration = new XDeviceConfiguration();
                XManager.DeviceConfiguration.RootDevice = new XDevice()
                {
                    DriverUID = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System).UID,
                    Driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System)
                };

                ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);

                //ServiceFactory.SaveService.Reset();
                //DevicesModule.ViewModels.DevicesViewModel.UpdateGuardVisibility();

                //DevicesModule.DevicesModule.CreateViewModels();
                //PlansModule.PlansModule.CreateViewModels();

                ServiceFactory.Layout.Close();
                ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);
            }
        }

        public bool CanChangeConfig
        {
            get { return (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Adm_ChangeConfigDevices)); }
        }

        void OnValidate(object sender, RoutedEventArgs e)
        {
            ServiceFactory.Layout.ShowValidationArea(null);
            var validationErrorsViewModel = new ValidationErrorsViewModel();
            if (validationErrorsViewModel.HasErrors)
            {
                ServiceFactory.Layout.ShowValidationArea(new ValidationErrorsViewModel());
            }
        }

        void OnSaveToFile(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "firesec2 files|*.fsc2";
            if (saveDialog.ShowDialog().Value)
                SaveToFile(CopyFrom(), saveDialog.FileName);
        }

        void OnLoadFromFile(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Filter = "firesec2 files|*.fsc2";
            if (openDialog.ShowDialog().Value)
            {
                CopyTo(LoadFromFile(openDialog.FileName));

                FiresecManager.UpdateConfiguration();
                ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);

                //DevicesModule.DevicesModule.CreateViewModels();
                ServiceFactory.Layout.Close();
                ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);
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
                    return (FullConfiguration)dataContractSerializer.ReadObject(fileStream);
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