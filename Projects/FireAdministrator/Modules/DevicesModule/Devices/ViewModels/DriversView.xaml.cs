using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;

namespace DevicesModule.ViewModels.Devices
{
    //DriversView driversView = new DriversView();
    //driversView.ShowDialog();

    public partial class DriversView : Window
    {
        public DriversView()
        {
            InitializeComponent();

            DataContext = this;

            var nullStates = new List<string>();

            foreach (var driver in Drivers)
            {
                foreach (var state in driver.States)
                {
                    if (string.IsNullOrEmpty(state.Code))
                    {
                        nullStates.Add(driver.ShortName + " - " + state.Name);
                    }
                }
            }

            var xxx = Drivers.FirstOrDefault(x => x.DriverName == "Группа");
        }

        public IEnumerable<Driver> Drivers
        {
            get
            {
                return from Driver driver in FiresecManager.Drivers
                       where driver.IsIgnore == false
                       select driver;
            }
        }
    }
}