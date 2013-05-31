using System;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
    [DataContract]
    public class XPumpStationPump
    {
        [DataMember]
        public Guid DeviceUID { get; set; }

        [DataMember]
        public XPumpStationPumpType PumpStationPumpType { get; set; }

        public XDevice Device { get; set; }
    }
}