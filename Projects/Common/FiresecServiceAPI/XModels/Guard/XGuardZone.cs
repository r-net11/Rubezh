using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class XGuardZone : XBase, IPlanPresentable
	{
		public XGuardZone()
		{
			PlanElementUIDs = new List<Guid>();
			GuardZoneDevices = new List<XGuardZoneDevice>();
		}

		[XmlIgnore]
		public override XBaseObjectType ObjectType { get { return XBaseObjectType.GuardZone; } }

		[XmlIgnore]
		public List<Guid> PlanElementUIDs { get; set; }

		[DataMember]
		public XGuardZoneEnterMethod GuardZoneEnterMethod { get; set; }

		[DataMember]
		public int SetGuardLevel { get; set; }

		[DataMember]
		public int ResetGuardLevel { get; set; }

		[DataMember]
		public int SetAlarmLevel { get; set; }

		[DataMember]
		public int Delay { get; set; }

		[DataMember]
		public int ResetDelay { get; set; }

		[DataMember]
		public int AlarmDelay { get; set; }

		[DataMember]
		public List<Guid> DeviceUIDs { get; set; }

		[DataMember]
		public List<XGuardZoneDevice> GuardZoneDevices { get; set; }
	}
}