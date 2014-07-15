using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.GK
{
	[DataContract]
	public class XGuardZone : XBase, INamedBase, IIdentity, IPlanPresentable
	{
		public XGuardZone()
		{
			UID = BaseUID;
			PlanElementUIDs = new List<Guid>();
			Devices = new List<XDevice>();
		}

		public override XBaseObjectType ObjectType { get { return XBaseObjectType.GuardZone; } }

		public List<XDevice> Devices { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public ushort No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Level { get; set; }

		[DataMember]
		public int Delay { get; set; }

		public override string PresentationName
		{
			get { return No + "." + Name; }
		}

		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}
		public event Action Changed;
		public List<Guid> PlanElementUIDs { get; set; }
	}
}