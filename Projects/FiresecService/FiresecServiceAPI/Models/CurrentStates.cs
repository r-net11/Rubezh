using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class CurrentStates
    {
        [DataMember]
        public List<DeviceState> DeviceStates { get; set; }

        [DataMember]
        public List<ZoneState> ZoneStates { get; set; }
    }
}
