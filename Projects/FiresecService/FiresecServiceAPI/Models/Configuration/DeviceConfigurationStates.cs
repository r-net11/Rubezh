using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class DeviceConfigurationStates
    {
        [DataMember]
        public List<DeviceState> DeviceStates { get; set; }

        [DataMember]
        public List<ZoneState> ZoneStates { get; set; }
    }
}