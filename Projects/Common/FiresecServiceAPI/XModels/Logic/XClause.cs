using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class XClause
	{
		public XClause()
		{
			StateType = XStateBit.Fire1;
			DeviceUIDs = new List<Guid>();
			ZoneUIDs = new List<Guid>();
			GuardZoneUIDs = new List<Guid>();
			DirectionUIDs = new List<Guid>();
			MPTUIDs = new List<Guid>();
			DelayUIDs = new List<Guid>();

			Devices = new List<XDevice>();
			Zones = new List<XZone>();
			GuardZones = new List<XGuardZone>();
			Directions = new List<XDirection>();
			MPTs = new List<XMPT>();
			Delays = new List<XDelay>();
		}

		[XmlIgnore]
		public List<XDevice> Devices { get; set; }
		[XmlIgnore]
		public List<XZone> Zones { get; set; }
		[XmlIgnore]
		public List<XGuardZone> GuardZones { get; set; }
		[XmlIgnore]
		public List<XDirection> Directions { get; set; }
		[XmlIgnore]
		public List<XMPT> MPTs { get; set; }
		[XmlIgnore]
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
		public List<Guid> GuardZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> DirectionUIDs { get; set; }

		[DataMember]
		public List<Guid> MPTUIDs { get; set; }

		[DataMember]
		public List<Guid> DelayUIDs { get; set; }

		[DataMember]
		public ClauseOperationType ClauseOperationType { get; set; }

		public bool HasObjects()
		{
			return Devices.Count > 0 || Zones.Count > 0 || GuardZones.Count > 0 || Directions.Count > 0 || MPTs.Count > 0 || Delays.Count > 0;
		}

		public static string ClauseToString(ClauseOperationType clauseOperationType, XStateBit stateBit)
		{
			switch (clauseOperationType)
			{
				case ClauseOperationType.AllZones:
				case ClauseOperationType.AnyZone:
					switch (stateBit)
					{
						case XStateBit.Fire1:
							return "Пожар 1";

						case XStateBit.Fire2:
							return "Пожар 2";

						case XStateBit.Attention:
							return "Внимание";
					}
					break;

				case ClauseOperationType.AllGuardZones:
				case ClauseOperationType.AnyGuardZone:
					switch (stateBit)
					{
						case XStateBit.On:
							return "Не на охране";

						case XStateBit.Off:
							return "На охране";

						case XStateBit.Attention:
							return "Сработка";
					}
					break;
			}

			return stateBit.ToDescription();
		}
	}
}