using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using System;

namespace SettingsModule.ViewModels
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

            Test();
        }

        public List<Driver> Drivers { get; private set; }

        void Test()
        {
            var nullStates = new List<string>();

            foreach (var driver in Drivers)
            {
                foreach (var state in driver.States)
                {
                    if (string.IsNullOrEmpty(state.Code))
                    {
                        nullStates.Add(String.Format("{0} - {1}", driver.ShortName, state.Name));
                    }
                }
            }

            var groupDevice = Drivers.FirstOrDefault(x => x.DriverName == "Группа");
        }
    }
}