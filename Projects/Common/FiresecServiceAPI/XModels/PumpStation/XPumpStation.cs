using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XPumpStation : XBase, IInputObjectsBase, INamedBase
	{
		public XPumpStation()
		{
			UID = BaseUID;
			Delay = 10;
			Hold = 60;
			DelayRegime = DelayRegime.Off;
			NSPumpsCount = 1;
			NSDeltaTime = 15;
			StartLogic = new XDeviceLogic();
			StopLogic = new XDeviceLogic();
			AutomaticOffLogic = new XDeviceLogic();

			InputDevices = new List<XDevice>();
			InputZones = new List<XZone>();
			InputDirections = new List<XDirection>();
			NSDevices = new List<XDevice>();
			NSDeviceUIDs = new List<Guid>();
		}

		public override XBaseObjectType ObjectType { get { return XBaseObjectType.PumpStation; } }

		public List<XDevice> InputDevices { get; set; }
		public List<XZone> InputZones { get; set; }
		public List<XDirection> InputDirections { get; set; }
		public List<XDevice> NSDevices { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public int No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public ushort Delay { get; set; }

		[DataMember]
		public ushort Hold { get; set; }

		[DataMember]
		public DelayRegime DelayRegime { get; set; }

		[DataMember]
		public int NSPumpsCount { get; set; }

		[DataMember]
		public int NSDeltaTime { get; set; }

		[DataMember]
		public List<Guid> NSDeviceUIDs { get; set; }

		[DataMember]
		public XDeviceLogic StartLogic { get; set; }

		[DataMember]
		public XDeviceLogic StopLogic { get; set; }

		[DataMember]
		public XDeviceLogic AutomaticOffLogic { get; set; }

		public override string PresentationName
		{
			get { return "0" + No + "." + Name; }
		}

		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}
		public event Action Changed;
	}
}