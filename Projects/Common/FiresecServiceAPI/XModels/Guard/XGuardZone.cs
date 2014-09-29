using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
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

		public XGuardZoneEnterMethod GuardZoneEnterMethod { get; set; }
		public int SetGuardLevel { get; set; }
		public int ResetGuardLevel { get; set; }
		public int SetAlarmLevel { get; set; }
		public int Delay { get; set; }
		public int ResetDelay { get; set; }
		public int AlarmDelay { get; set; }
		public List<Guid> DeviceUIDs { get; set; }
		public List<XGuardZoneDevice> GuardZoneDevices { get; set; }
	}
}