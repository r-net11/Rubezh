using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ServiceApi
{
    [DataContract]
    public class ZoneState
    {
        [DataMember]
        public string No { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public bool ZoneChanged { get; set; }
    }
}
