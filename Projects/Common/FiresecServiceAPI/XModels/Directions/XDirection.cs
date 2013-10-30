using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XDirection : XBinaryBase
	{
		public XDirection()
		{
            UID = Guid.NewGuid();
            DirectionZones = new List<XDirectionZone>();
            DirectionDevices = new List<XDirectionDevice>();
			Regime = 1;
			InputZones = new List<XZone>();
			InputDevices = new List<XDevice>();
            OutputDevices = new List<XDevice>();
			NSDevices = new List<XDevice>();
			PlanElementUIDs = new List<Guid>();
			NSDeviceUIDs = new List<Guid>();
			NSPumpsCount = 1;
			NSDeltaTime = 5;
		}
		public XDirectionState DirectionState { get; set; }
		public override XBaseState GetXBaseState() { return DirectionState; }
		public List<XZone> InputZones { get; set; }
		public List<XDevice> InputDevices { get; set; }
        public List<XDevice> OutputDevices { get; set; }
		public List<XDevice> NSDevices { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public ushort No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public ushort Delay { get; set; }

		[DataMember]
		public ushort Hold { get; set; }

		[DataMember]
		public ushort Regime { get; set; }

		[DataMember]
        public List<XDirectionZone> DirectionZones { get; set; }

		[DataMember]
        public List<XDirectionDevice> DirectionDevices { get; set; }

		[DataMember]
		public bool IsOPCUsed { get; set; }

		[DataMember]
		public bool IsNS { get; set; }

		[DataMember]
		public List<Guid> NSDeviceUIDs { get; set; }

		[DataMember]
		public int NSPumpsCount { get; set; }

		[DataMember]
		public int NSDeltaTime { get; set; }

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
					Type = "Направление",
					Name = Name,
					Address = No.ToString()
				};
			}
		}

		public override string GetBinaryDescription()
		{
			return PresentationName;
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