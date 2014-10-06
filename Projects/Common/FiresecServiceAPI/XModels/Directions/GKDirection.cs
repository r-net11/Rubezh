using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKDirection : GKBase, IPlanPresentable
	{
		public GKDirection()
		{
			DirectionZones = new List<GKDirectionZone>();
			DirectionDevices = new List<GKDirectionDevice>();
			DelayRegime = DelayRegime.Off;

			InputDevices = new List<GKDevice>();
			InputZones = new List<GKZone>();
			OutputDevices = new List<GKDevice>();
			PlanElementUIDs = new List<Guid>();
		}

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Direction; } }

		[XmlIgnore]
		public List<GKDevice> InputDevices { get; set; }
		[XmlIgnore]
		public List<GKZone> InputZones { get; set; }
		[XmlIgnore]
		public List<GKDevice> OutputDevices { get; set; }

		[DataMember]
		public ushort Delay { get; set; }

		[DataMember]
		public ushort Hold { get; set; }

		[DataMember]
		public DelayRegime DelayRegime { get; set; }

		[DataMember]
		public List<GKDirectionZone> DirectionZones { get; set; }

		[DataMember]
		public List<GKDirectionDevice> DirectionDevices { get; set; }

		[DataMember]
		public bool IsOPCUsed { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }
	}
}