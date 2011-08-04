using System.Windows;
using System.Windows.Controls;
using System.Linq;
using FiresecClient;
using Microsoft.Win32;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Events;

namespace FireAdministrator
{
    public partial class MenuView : UserControl
    {
        public MenuView()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OnSetNewConfig(object sender, RoutedEventArgs e)
        {
            //FiltersModule.FiltersModule.Save();
            //SoundsModule.SoundsModule.Save();
            FiresecManager.SetConfiguration();
        }

        private void OnCreateNew(object sender, RoutedEventArgs e)
        {

        }

        public bool CanChangeConfig
        {
            get { return (FiresecManager.CurrentPermissions.Any(x => x.PermissionType == PermissionType.Adm_ChangeConfigDevices)); }
        }

        private void OnLoadFromFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "firesec2 files|*.fsc2";
            if (openDialog.ShowDialog().Value)
            {
                FiresecManager.LoadFromFile(openDialog.FileName);
            }

            DevicesModule.DevicesModule.CreateViewModels();
            ServiceFactory.Layout.Close();
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(null);
        }

        private void OnSaveToFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "firesec2 files|*.fsc2";
            if (saveDialog.ShowDialog().Value)
            {
                FiresecManager.SaveToFile(saveDialog.FileName);
            }
        }

        private void OnValidate(object sender, RoutedEventArgs e)
        {
            DevicesModule.DevicesModule.Validate();
            FiltersModule.FiltersModule.Validate();
        }
    }
}
