using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace XFiresecAPI
{
	[DataContract]
	public class XPumpStation : XBase
	{
		public XPumpStation()
		{
			UID = BaseUID;
			DirectionZones = new List<XDirectionZone>();
			DirectionDevices = new List<XDirectionDevice>();
			Regime = 1;
			InputZones = new List<XZone>();
			InputDevices = new List<XDevice>();
			NSDevices = new List<XDevice>();
			NSDeviceUIDs = new List<Guid>();
			NSPumpsCount = 1;
			NSDeltaTime = 5;
			StartLogic = new XDeviceLogic();
			ForbidLogic = new XDeviceLogic();
		}
		public XDirectionState DirectionState { get; set; }
		public override XBaseState GetXBaseState() { return DirectionState; }
		public List<XZone> InputZones { get; set; }
		public List<XDevice> InputDevices { get; set; }
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
		public List<Guid> NSDeviceUIDs { get; set; }

		[DataMember]
		public int NSPumpsCount { get; set; }

		[DataMember]
		public int NSDeltaTime { get; set; }

		[DataMember]
		public XDeviceLogic StartLogic { get; set; }

		[DataMember]
		public XDeviceLogic ForbidLogic { get; set; }

		public override string PresentationName
		{
			get { return No + "." + Name; }
		}

		public override string DescriptorInfo
		{
			get { return "НС " + PresentationName; }
		}

		public override string GetDescriptorName()
		{
			return PresentationName;
		}

		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}
		public event Action Changed;
	}
}