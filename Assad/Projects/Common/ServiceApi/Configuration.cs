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
        public ComServer.CoreConfig.config CoreConfig { get; set; }

        [IgnoreDataMember]
        public ComServer.Metadata.config Metadata { get; set; }
    }
}
