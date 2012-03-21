using System.Runtime.Serialization;
using System;
using System.Collections.Generic;

namespace GroupControllerModule.Models
{
    [DataContract]
    public class XZone
    {
        public XZone()
        {
            DetectorCount = 2;
            DeviceUIDs = new List<Guid>();
            KAUDevices = new List<XDevice>();
        }

        public List<XDevice> KAUDevices { get; set; }
        public short InternalKAUNo { get; set; }

        [DataMember]
        public ulong No { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int DetectorCount { get; set; }

        [DataMember]
        public List<Guid> DeviceUIDs { get; set; }

        public string PresentationName
        {
            get { return No + "." + Name; }
        }
    }
}