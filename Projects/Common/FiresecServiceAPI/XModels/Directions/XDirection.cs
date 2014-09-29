using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	public class XDirection : XBase, IPlanPresentable
	{
		public XDirection()
		{
			DirectionZones = new List<XDirectionZone>();
			DirectionDevices = new List<XDirectionDevice>();
			DelayRegime = DelayRegime.Off;

			InputDevices = new List<XDevice>();
			InputZones = new List<XZone>();
			OutputDevices = new List<XDevice>();
			PlanElementUIDs = new List<Guid>();
		}

		[XmlIgnore]
		public override XBaseObjectType ObjectType { get { return XBaseObjectType.Direction; } }

		[XmlIgnore]
		public List<XDevice> InputDevices { get; set; }
		[XmlIgnore]
		public List<XZone> InputZones { get; set; }
		[XmlIgnore]
		public List<XDevice> OutputDevices { get; set; }

		public ushort Delay { get; set; }
		public ushort Hold { get; set; }
		public DelayRegime DelayRegime { get; set; }
		public List<XDirectionZone> DirectionZones { get; set; }
		public List<XDirectionDevice> DirectionDevices { get; set; }
		public bool IsOPCUsed { get; set; }
		public List<Guid> PlanElementUIDs { get; set; }
	}
}