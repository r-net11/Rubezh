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

namespace FireAdministrator.Views
{
    public partial class MenuView : UserControl
    {
        public MenuView()
        {
            InitializeComponent();
            DataContextChanged += new DependencyPropertyChangedEventHandler(MenuView_DataContextChanged);
            ServiceFactory.SaveService.Changed += new Action(SaveService_Changed);
        }

        void MenuView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is MenuViewModel)
                ((MenuViewModel)e.NewValue).SetNewConfigEvent += (s, ee) => { ee.Cancel = !SetNewConfig(); };
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

        public bool SetNewConfig()
        {
            if (CanChangeConfig == false)
            {
                MessageBoxService.Show("У вас нет прав на сохранение конфигурации");
                return false;
            }

            ServiceFactory.Events.GetEvent<ConfigurationSavingEvent>().Publish(null);

            var validationResult = ServiceFactory.ValidationService.Validate();
            if (validationResult.HasErrors)
            {
                if (validationResult.CannotSave)
                {
                    MessageBoxService.ShowWarning("Обнаружены ошибки. Операция прервана");
                    return false;
                }

                if (MessageBoxService.ShowQuestion("Конфигурация содержит ошибки. Продолжить?") != MessageBoxResult.Yes)
                    return false;
            }

            WaitHelper.Execute(() =>
            {
                if (ServiceFactory.SaveService.DevicesChanged)
                {
                    var result = FiresecManager.FiresecService.SetDeviceConfiguration(FiresecManager.FiresecConfiguration.DeviceConfiguration);
                    if (result.HasError)
                    {
                        MessageBoxService.ShowError(result.Error);
                    }
                }

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
            });
            ServiceFactory.SaveService.Reset();
            _saveButton.IsEnabled = false;
            return true;
        }

        void OnCreateNew(object sender, RoutedEventArgs e)
        {
            var result = MessageBoxService.ShowQuestion("Вы уверены, что хотите создать новую конфигурацию");
            if (result == MessageBoxResult.Yes)
            {
                FiresecManager.FiresecConfiguration.DeviceConfiguration = new DeviceConfiguration();
                FiresecManager.PlansConfiguration = new PlansConfiguration();
                FiresecManager.SystemConfiguration = new SystemConfiguration();

                FiresecManager.FiresecConfiguration.DeviceConfiguration.RootDevice = new Device()
                {
                    DriverUID = FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Computer).UID,
                    Driver = FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Computer)
                };
                FiresecManager.FiresecConfiguration.DeviceConfiguration.Update();
                FiresecManager.PlansConfiguration.Update();

                XManager.SetEmptyConfiguration();

                ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);

                ServiceFactory.SaveService.DevicesChanged = true;
                ServiceFactory.SaveService.PlansChanged = true;
                ServiceFactory.SaveService.InstructionsChanged = true;
                ServiceFactory.SaveService.SoundsChanged = true;
                ServiceFactory.SaveService.FilterChanged = true;
                ServiceFactory.SaveService.CamerasChanged = true;
                ServiceFactory.SaveService.XDevicesChanged = true;

                ServiceFactory.Layout.Close();
                ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);
                ServiceFactory.Layout.ShowFooter(null);
            }
        }

        public bool CanChangeConfig
        {
            get { return (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Adm_ChangeConfigDevices)); }
        }

        void OnValidate(object sender, RoutedEventArgs e)
        {
            ServiceFactory.ValidationService.Validate();
        }

        void OnSaveToFile(object sender, RoutedEventArgs e)
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

        void OnLoadFromFile(object sender, RoutedEventArgs e)
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

        FullConfiguration CopyFrom()
        {
            return new FullConfiguration()
            {
                DeviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration,
                LibraryConfiguration = FiresecManager.LibraryConfiguration,
                PlansConfiguration = FiresecManager.PlansConfiguration,
                SecurityConfiguration = FiresecManager.SecurityConfiguration,
                SystemConfiguration = FiresecManager.SystemConfiguration,
                XDeviceConfiguration = XManager.DeviceConfiguration,
                XDriversConfiguration = XManager.DriversConfiguration,
                Version = new ConfigurationVersion() { MajorVersion = 1, MinorVersion = 1 }
            };
        }

        void CopyTo(FullConfiguration fullConfiguration)
        {
            FiresecManager.FiresecConfiguration.DeviceConfiguration = fullConfiguration.DeviceConfiguration;
            FiresecManager.LibraryConfiguration = fullConfiguration.LibraryConfiguration;
            FiresecManager.PlansConfiguration = fullConfiguration.PlansConfiguration;
            FiresecManager.SecurityConfiguration = fullConfiguration.SecurityConfiguration;
            FiresecManager.SystemConfiguration = fullConfiguration.SystemConfiguration;
            XManager.DeviceConfiguration = fullConfiguration.XDeviceConfiguration;
            XManager.DriversConfiguration = fullConfiguration.XDriversConfiguration;
        }

        FullConfiguration LoadFromFile(string fileName)
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