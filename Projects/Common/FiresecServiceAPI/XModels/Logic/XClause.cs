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
			ClauseJounOperationType = ClauseJounOperationType.Or;
            StateType = XStateType.Fire1;
			ZoneUIDs = new List<Guid>();
            DeviceUIDs = new List<Guid>();
			DirectionUIDs = new List<Guid>();

			Zones = new List<XZone>();
			Devices = new List<XDevice>();
			Directions = new List<XDirection>();
        }

		public List<XZone> Zones { get; set; }
		public List<XDevice> Devices { get; set; }
		public List<XDirection> Directions { get; set; }

        [DataMember]
        public ClauseConditionType ClauseConditionType { get; set; }

        [DataMember]
        public XStateType StateType { get; set; }

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

        [DataMember]
        public List<Guid> DeviceUIDs { get; set; }

		[DataMember]
		public List<Guid> DirectionUIDs { get; set; }

        [DataMember]
        public ClauseOperationType ClauseOperationType { get; set; }

        [DataMember]
        public ClauseJounOperationType ClauseJounOperationType { get; set; }
    }
}