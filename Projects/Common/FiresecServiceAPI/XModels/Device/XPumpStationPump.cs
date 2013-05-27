using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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