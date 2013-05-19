using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XZone : XBinaryBase
	{
		public XZone()
		{
            UID = Guid.NewGuid();
			Fire1Count = 2;
			Fire2Count = 3;
			Devices = new List<XDevice>();
			Directions = new List<XDirection>();
            DevicesInLogic = new List<XDevice>();
			PlanElementUIDs = new List<Guid>();
		}

		public XZoneState ZoneState { get; set; }
		public override XBaseState GetXBaseState() { return ZoneState; }
		public List<XDevice> Devices { get; set; }
		public List<XDirection> Directions { get; set; }
        public List<XDevice> DevicesInLogic { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public ushort No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public ushort Fire1Count { get; set; }

		[DataMember]
		public ushort Fire2Count { get; set; }

		public string PresentationName
		{
			get { return No + "." + Name; }
		}

		public override XBinaryInfo BinaryInfo
		{
			get
			{
				return new XBinaryInfo()
				{
					Type = "Зона",
					Name = Name,
					Address = No.ToString()
				};
			}
		}

		public override string GetBinaryDescription()
		{
			return Name + " - " + No.ToString();
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