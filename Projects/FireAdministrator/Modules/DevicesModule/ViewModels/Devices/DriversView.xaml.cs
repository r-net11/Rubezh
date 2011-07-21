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

            List<string> nullStates = new List<string>();

            foreach (var driver in Drivers)
            {
                foreach (var state in driver.States)
                {
                    if (string.IsNullOrEmpty(state.code))
                    {
                        nullStates.Add(driver.ShortName + " - " + state.name);
                    }
                }
            }
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
