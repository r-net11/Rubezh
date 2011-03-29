using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ServiceApi
{
    [DataContract]
    public class Zone
    {
        [DataMember]
        public string No { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DetectorCount { get; set; }

        [DataMember]
        public string EvacuationTime { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string ValidationError { get; set; }
    }
}
