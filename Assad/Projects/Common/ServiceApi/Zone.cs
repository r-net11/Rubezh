using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ServiceApi
{
    [DataContract(IsReference = true)]
    public class Zone
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string DetectorCount { get; set; }

        [DataMember]
        public string EvacuationTime { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string ValidationError { get; set; }

        [DataMember]
        public List<Device> Devices { get; set; }

        [DataMember]
        public string State { get; set; }
    }
}
