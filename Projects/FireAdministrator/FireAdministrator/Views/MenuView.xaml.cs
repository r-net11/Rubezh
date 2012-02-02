using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;
using Microsoft.Win32;
using System.Collections.Generic;
using Controls.MessageBox;

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
            SetNewConfig();
        }

        public void SetNewConfig()
        {
            PlansModule.PlansModule.Save();

            if (ServiceFactory.SaveService.DevicesChanged)
                FiresecManager.FiresecService.SetDeviceConfiguration(FiresecManager.DeviceConfiguration);

            if (ServiceFactory.SaveService.PlansChanged)
                FiresecManager.FiresecService.SetPlansConfiguration(FiresecManager.PlansConfiguration);

            if (ServiceFactory.SaveService.SecurityChanged)
                FiresecManager.FiresecService.SetSecurityConfiguration(FiresecManager.SecurityConfiguration);

            if (ServiceFactory.SaveService.LibraryChanged)
                FiresecManager.FiresecService.SetLibraryConfiguration(FiresecManager.LibraryConfiguration);

            if ((ServiceFactory.SaveService.InstructionsChanged) || (ServiceFactory.SaveService.SoundsChanged) || (ServiceFactory.SaveService.FilterChanged))
                FiresecManager.FiresecService.SetSystemConfiguration(FiresecManager.SystemConfiguration);

            ServiceFactory.SaveService.Reset();
            _saveButton.IsEnabled = false;
        }

        void OnCreateNew(object sender, RoutedEventArgs e)
        {
            var result = MessageBoxService.ShowQuestion("Вы уверены, что хотите создать новую конфигурацию");
            if (result == MessageBoxResult.Yes)
            {
                FiresecManager.DeviceConfiguration.Devices = new List<Device>();
                FiresecManager.DeviceConfiguration.Zones = new List<Zone>();
                FiresecManager.DeviceConfiguration.Directions = new List<Direction>();

                var device = new Device()
                {
                    DriverUID = FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Computer).UID,
                    Driver =    FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Computer)
                };
                FiresecManager.DeviceConfiguration.RootDevice = device;
                FiresecManager.DeviceConfiguration.Update();

                FiresecManager.PlansConfiguration.Plans = new List<Plan>();
                FiresecManager.PlansConfiguration.Update();

                ServiceFactory.SaveService.DevicesChanged = true;
                ServiceFactory.SaveService.PlansChanged = true;
                DevicesModule.ViewModels.DevicesViewModel.UpdateGuardVisibility();

                DevicesModule.DevicesModule.CreateViewModels();
                PlansModule.PlansModule.CreateViewModels();

                ServiceFactory.Layout.Close();
                ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);
            }
        }

        public bool CanChangeConfig
        {
            get { return (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Adm_ChangeConfigDevices)); }
        }

        void OnLoadFromFile(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Filter = "firesec2 files|*.fsc2";
            if (openDialog.ShowDialog().Value)
                FiresecManager.LoadFromFile(openDialog.FileName);

            DevicesModule.DevicesModule.CreateViewModels();
            ServiceFactory.Layout.Close();
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);

            ServiceFactory.SaveService.DevicesChanged = true;
        }

        void OnSaveToFile(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "firesec2 files|*.fsc2";
            if (saveDialog.ShowDialog().Value)
                FiresecManager.SaveToFile(saveDialog.FileName);
        }

        void OnValidate(object sender, RoutedEventArgs e)
        {
            ServiceFactory.Layout.ShowValidationArea(new ValidationErrorsViewModel());
        }
    }
}