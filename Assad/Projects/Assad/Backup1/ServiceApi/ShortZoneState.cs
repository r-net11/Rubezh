using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ServiceApi
{
    [DataContract]
    public class ShortZoneState
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string State { get; set; }
    }
}
