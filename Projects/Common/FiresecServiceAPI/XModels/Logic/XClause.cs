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
			StateType = XStateBit.Fire1;
			DeviceUIDs = new List<Guid>();
			ZoneUIDs = new List<Guid>();
			DirectionUIDs = new List<Guid>();
			MPTUIDs = new List<Guid>();
			DelayUIDs = new List<Guid>();

			Devices = new List<XDevice>();
			Zones = new List<XZone>();
			Directions = new List<XDirection>();
			MPTs = new List<XMPT>();
			Delays = new List<XDelay>();
		}

		public List<XDevice> Devices { get; set; }
		public List<XZone> Zones { get; set; }
		public List<XDirection> Directions { get; set; }
		public List<XMPT> MPTs { get; set; }
		public List<XDelay> Delays { get; set; }

		[DataMember]
		public ClauseConditionType ClauseConditionType { get; set; }

		[DataMember]
		public XStateBit StateType { get; set; }

		[DataMember]
		public List<Guid> DeviceUIDs { get; set; }

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> DirectionUIDs { get; set; }

		[DataMember]
		public List<Guid> MPTUIDs { get; set; }

		[DataMember]
		public List<Guid> DelayUIDs { get; set; }

		[DataMember]
		public ClauseOperationType ClauseOperationType { get; set; }

		[DataMember]
		public ClauseJounOperationType ClauseJounOperationType { get; set; }

		public bool HasObjects()
		{
			return Devices.Count > 0 || Zones.Count > 0 || Directions.Count > 0 || MPTs.Count > 0 || Delays.Count > 0;
		}
	}
}