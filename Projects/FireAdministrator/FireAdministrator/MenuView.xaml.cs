using System.Windows;
using System.Windows.Controls;
using FiresecClient;

namespace FireAdministrator
{
    public partial class MenuView : UserControl
    {
        public MenuView()
        {
            InitializeComponent();
        }

        private void OnSetNewConfig(object sender, RoutedEventArgs e)
        {
            FiresecManager.SetNewConfig();
        }

        private void OnCreateNew(object sender, RoutedEventArgs e)
        {

        }

        private void OnLoadFromFile(object sender, RoutedEventArgs e)
        {
            FiresecManager.LoadFromFile("D:/Projects/3rdParty/Firesec/Bisc.fsc");
        }

        private void OnSaveToFile(object sender, RoutedEventArgs e)
        {
            FiresecManager.SaveToFile("D:/config.fsc");
        }

        private void OnValidate(object sender, RoutedEventArgs e)
        {

        }
    }
}
