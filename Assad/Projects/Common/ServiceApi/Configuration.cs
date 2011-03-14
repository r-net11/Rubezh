using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ServiceApi
{
    [DataContract(IsReference = true)]
    public class Configuration
    {
        [DataMember]
        public List<Device> Devices { get; set; }

        [DataMember]
        public List<Zone> Zones { get; set; }

        [IgnoreDataMember]
        public Firesec.CoreConfig.config CoreConfig { get; set; }

        [IgnoreDataMember]
        public Firesec.Metadata.config Metadata { get; set; }
    }
}
