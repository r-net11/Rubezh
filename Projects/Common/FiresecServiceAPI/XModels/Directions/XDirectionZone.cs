using System;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
    [DataContract]
    public class XDirectionZone
    {
        public XZone Zone { get; set; }

        [DataMember]
        public Guid ZoneUID { get; set; }

        [DataMember]
        public XStateType StateType { get; set; }
    }
}