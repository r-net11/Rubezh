using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        }

        void OnCreateNew(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите создать новую конфигурацию", "Новая конфигурация", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                FiresecManager.DeviceConfiguration.Devices = new System.Collections.Generic.List<Device>();
                FiresecManager.DeviceConfiguration.Zones = new System.Collections.Generic.List<Zone>();
                FiresecManager.DeviceConfiguration.Directions = new System.Collections.Generic.List<Direction>();

                Device device = new Device();
                device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.DriverName == "Компьютер");
                device.DriverId = device.Driver.Id;
                FiresecManager.DeviceConfiguration.RootDevice = device;
                FiresecManager.DeviceConfiguration.Update();

                DevicesModule.DevicesModule.CreateViewModels();
                ServiceFactory.Layout.Close();
                ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(null);

                DevicesModule.DevicesModule.HasChanges = true;
            }
        }

        public bool CanChangeConfig
        {
            get { return (FiresecManager.CurrentPermissions.Any(x => x.PermissionType == PermissionType.Adm_ChangeConfigDevices)); }
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
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(null);

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
            DevicesModule.DevicesModule.Validate();
            FiltersModule.FilterModule.Validate();
            //InstructionsModule.InstructionsModule.Validate();
        }
    }
}