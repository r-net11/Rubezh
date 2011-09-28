using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;

namespace RepFileManager
{
    public static class Helper
    {
        public static List<StateType> AllStates
        {
            get { return new List<StateType>(Enum.GetValues(typeof(StateType)).Cast<StateType>()); }
        }

        public static List<Driver> CommonPanelDrivers
        {
            get
            {
                var drivers = new List<Driver>();
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.BUNS));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Rubezh_10AM));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Rubezh_2AM));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Rubezh_2OP));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Rubezh_4A));
                return drivers;
            }
        }

        public static List<Driver> CommonCommunicationDrivers
        {
            get
            {
                var drivers = new List<Driver>();
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.MS_1));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.MS_2));
                return drivers;
            }
        }
    }
}
