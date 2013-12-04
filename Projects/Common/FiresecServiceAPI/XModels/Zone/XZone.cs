using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XZone : XBase
	{
		public XZone()
		{
			UID = BaseUID;// Guid.NewGuid();
			Fire1Count = 2;
			Fire2Count = 3;
			Devices = new List<XDevice>();
			Directions = new List<XDirection>();
            DevicesInLogic = new List<XDevice>();
			PlanElementUIDs = new List<Guid>();
		}

		public XZoneState InternalState { get; set; }
		public override XBaseState BaseState
		{
			get { return InternalState; }
		}

		public List<XDevice> Devices { get; set; }
		public List<XDirection> Directions { get; set; }
        public List<XDevice> DevicesInLogic { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public int No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Fire1Count { get; set; }

		[DataMember]
		public int Fire2Count { get; set; }

		[DataMember]
		public bool IsOPCUsed { get; set; }

		public override string PresentationName
		{
			get { return No + "." + Name; }
		}

		public override string DescriptorInfo
		{
			get { return "Зона " + PresentationName; }
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