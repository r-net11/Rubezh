using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
    }
}
