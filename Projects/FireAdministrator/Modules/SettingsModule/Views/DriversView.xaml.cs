using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;

namespace SettingsModule.Views
{
    public partial class DriversView : Window
    {
        public DriversView()
        {
            InitializeComponent();

            Drivers = (from Driver driver in FiresecManager.Drivers
                       where driver.IsIgnore == false
                       select driver).ToList();

            DataContext = this;
        }

        public List<Driver> Drivers { get; private set; }
    }
}