using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ServiceApi
{
    [DataContract]
    public class StateConfiguration
    {
        [DataMember]
        public ShortDevice RootShortDevice { get; set; }

        [DataMember]
        public List<ShortZone> ShortZones { get; set; }

        [DataMember]
        public Firesec.Metadata.config Metadata { get; set; }
    }
}
