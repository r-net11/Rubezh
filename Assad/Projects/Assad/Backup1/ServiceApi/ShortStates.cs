using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ServiceApi
{
    [DataContract]
    public class ShortStates
    {
        [DataMember]
        public List<ShortDeviceState> ShortDeviceStates { get; set; }

        [DataMember]
        public List<ShortZoneState> ShortZoneStates { get; set; }
    }
}
