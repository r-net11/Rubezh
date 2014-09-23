using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.GK
{
	[DataContract]
	public class XZone : XBase, IPlanPresentable
	{
		public XZone()
		{
			Fire1Count = 2;
			Fire2Count = 3;
			PlanElementUIDs = new List<Guid>();
			Devices = new List<XDevice>();
			Directions = new List<XDirection>();
			DevicesInLogic = new List<XDevice>();
		}

		public override XBaseObjectType ObjectType { get { return XBaseObjectType.Zone; } }

		public List<XDevice> Devices { get; set; }
		public List<XDirection> Directions { get; set; }
		public List<XDevice> DevicesInLogic { get; set; }

		[DataMember]
		public int Fire1Count { get; set; }

		[DataMember]
		public int Fire2Count { get; set; }

		[DataMember]
		public bool IsOPCUsed { get; set; }

		public List<Guid> PlanElementUIDs { get; set; }
	}
}