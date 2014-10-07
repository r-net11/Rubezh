using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKGuardZone : GKBase, IPlanPresentable
	{
		public GKGuardZone()
		{
			PlanElementUIDs = new List<Guid>();
			GuardZoneDevices = new List<GKGuardZoneDevice>();
		}

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.GuardZone; } }

		[XmlIgnore]
		public List<Guid> PlanElementUIDs { get; set; }

		[DataMember]
		public GKGuardZoneEnterMethod GuardZoneEnterMethod { get; set; }

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
		public List<GKGuardZoneDevice> GuardZoneDevices { get; set; }
	}
}