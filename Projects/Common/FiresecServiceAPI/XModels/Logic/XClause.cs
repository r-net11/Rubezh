using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
    [DataContract]
    public class XClause
    {
        public XClause()
        {
            StateType = XStateType.Fire1;
            DeviceUIDs = new List<Guid>();
            ZoneUIDs = new List<Guid>();

			Devices = new List<XDevice>();
			Zones = new List<XZone>();
        }

		public List<XDevice> Devices { get; set; }
		public List<XZone> Zones { get; set; }

        [DataMember]
        public ClauseConditionType ClauseConditionType { get; set; }

        [DataMember]
        public XStateType StateType { get; set; }

        [DataMember]
        public List<Guid> DeviceUIDs { get; set; }

        [DataMember]
        public List<Guid> ZoneUIDs { get; set; }

        [DataMember]
        public ClauseOperationType ClauseOperationType { get; set; }

        [DataMember]
        public ClauseJounOperationType ClauseJounOperationType { get; set; }
    }
}