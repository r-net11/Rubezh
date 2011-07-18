using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecClient;
using FiresecClient.Models;

namespace DevicesModule.ViewModels.Devices
{
    public partial class DriversView : Window
    {
        public DriversView()
        {
            InitializeComponent();

            DataContext = this;
        }

        public IEnumerable<Driver> Drivers
        {
            get
            {
                return from Driver driver in FiresecManager.Configuration.Drivers
                       where driver.IsIgnore == false
                       select driver;
            }
        }
    }
}
