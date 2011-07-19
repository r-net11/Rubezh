using System.Windows;
using System.Windows.Controls;
using System.Linq;
using FiresecClient;
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

        private void OnSetNewConfig(object sender, RoutedEventArgs e)
        {
            FiresecManager.SetNewConfig();
        }

        private void OnCreateNew(object sender, RoutedEventArgs e)
        {

        }

        public bool CanChangeConfig
        {
            get
            {
                return (FiresecManager.CurrentPermissions.Any(x => x.PermissionType == FiresecClient.Models.PermissionType.Adm_ChangeConfigDevices));
            }
        }

        private void OnLoadFromFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "firesec files|*.fsc";
            if (openDialog.ShowDialog().Value)
            {
                FiresecManager.LoadFromFile(openDialog.FileName);
            }
        }

        private void OnSaveToFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "firesec files|*.fsc";
            if (saveDialog.ShowDialog().Value)
            {
                FiresecManager.SaveToFile(saveDialog.FileName);
            }
        }

        private void OnValidate(object sender, RoutedEventArgs e)
        {
            DevicesModule.DevicesModule.Validate();
            JournalModule.JournalModule.Validate();
        }
    }
}
