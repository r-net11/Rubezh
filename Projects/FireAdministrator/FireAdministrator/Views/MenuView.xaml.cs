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

namespace FireAdministrator
{
    public partial class MenuView : UserControl
    {
        public MenuView()
        {
            InitializeComponent();
            DataContext = this;
        }

        void OnSetNewConfig(object sender, RoutedEventArgs e)
        {
            FiresecManager.SetConfiguration();

            SoundsModule.SoundsModule.HasChanges = false;
            DevicesModule.DevicesModule.HasChanges = false;
            FiltersModule.FilterModule.HasChanges = false;
            LibraryModule.LibraryModule.HasChanges = false;
            InstructionsModule.InstructionsModule.HasChanges = false;
            SecurityModule.SecurityModule.HasChanges = false;
        }

        void OnCreateNew(object sender, RoutedEventArgs e)
        {
            var result = DialogBox.DialogBox.Show("Вы уверены, что хотите создать новую конфигурацию", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                FiresecManager.DeviceConfiguration.Devices = new System.Collections.Generic.List<Device>();
                FiresecManager.DeviceConfiguration.Zones = new System.Collections.Generic.List<Zone>();
                FiresecManager.DeviceConfiguration.Directions = new System.Collections.Generic.List<Direction>();

                Device device = new Device();
                device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Computer);
                device.DriverUID = device.Driver.UID;
                FiresecManager.DeviceConfiguration.RootDevice = device;
                FiresecManager.DeviceConfiguration.Update();

                DevicesModule.DevicesModule.CreateViewModels();
                ServiceFactory.Layout.Close();
                ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);

                DevicesModule.DevicesModule.HasChanges = true;
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
            {
                FiresecManager.LoadFromFile(openDialog.FileName);
            }

            DevicesModule.DevicesModule.CreateViewModels();
            ServiceFactory.Layout.Close();
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);

            DevicesModule.DevicesModule.HasChanges = true;
        }

        void OnSaveToFile(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "firesec2 files|*.fsc2";
            if (saveDialog.ShowDialog().Value)
            {
                FiresecManager.SaveToFile(saveDialog.FileName);
            }
        }

        void OnValidate(object sender, RoutedEventArgs e)
        {
            var validationErrorsViewModel = new ValidationErrorsViewModel();
            ServiceFactory.Layout.ShowValidationArea(validationErrorsViewModel);
        }
    }
}