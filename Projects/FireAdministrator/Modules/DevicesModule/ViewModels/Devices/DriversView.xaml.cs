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
using System.Windows.Shapes;
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
