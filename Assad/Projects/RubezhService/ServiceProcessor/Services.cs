using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceApi;

namespace ServiseProcessor
{
    public static class Services
    {
        public static CurrentConfiguration CurrentConfiguration { get; set; }
        public static ServiceApi.CurrentStates CurrentStates { get; set; }

        public static List<ServiceApi.Device> AllDevices { get; set; }

        public static Firesec.CoreConfig.config CoreConfig { get; set; }
    }
}
