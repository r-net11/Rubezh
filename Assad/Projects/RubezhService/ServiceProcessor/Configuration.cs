using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ServiseProcessor
{
    public class Configuration
    {
        //public List<Device> Devices { get; set; }
        //public List<Zone> Zones { get; set; }

        public List<ServiceApi.ShortDevice> ShortDevices { get; set; }

        public Firesec.CoreConfig.config CoreConfig { get; set; }
        public Firesec.Metadata.config Metadata { get; set; }

        public ServiceApi.StateConfiguration StateConfiguration { get; set; }
        public ServiceApi.ShortStates ShortStates { get; set; }
    }
}
