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
            Devices = new List<Guid>();
            Zones = new List<Guid>();

			XDevices = new List<XDevice>();
			XZones = new List<XZone>();
        }

		public List<XDevice> XDevices { get; set; }
		public List<XZone> XZones { get; set; }

        [DataMember]
        public XStateType StateType { get; set; }

        [DataMember]
        public List<Guid> Devices { get; set; }

        [DataMember]
        public List<Guid> Zones { get; set; }

        [DataMember]
        public ClauseOperandType ClauseOperandType { get; set; }

        [DataMember]
        public ClauseOperationType ClauseOperationType { get; set; }

        [DataMember]
        public ClauseJounOperationType ClauseJounOperationType { get; set; }
    }
}