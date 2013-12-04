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
			Delay = 10;
			Hold = 60;
			Regime = 2;
			NSPumpsCount = 1;
			NSDeltaTime = 5;
			StartLogic = new XDeviceLogic();
			ForbidLogic = new XDeviceLogic();
			StopLogic = new XDeviceLogic();

			InputDevices = new List<XDevice>();
			InputZones = new List<XZone>();
			InputDirections = new List<XDirection>();
			NSDevices = new List<XDevice>();
			NSDeviceUIDs = new List<Guid>();
			PumpStationState = new XPumpStationState()
			{
				PumpStation = this
			};
		}
		public XPumpStationState PumpStationState { get; set; }
		public override XBaseState GetXBaseState() { return PumpStationState; }
		public List<XDevice> InputDevices { get; set; }
		public List<XZone> InputZones { get; set; }
		public List<XDirection> InputDirections { get; set; }
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
		public int NSPumpsCount { get; set; }

		[DataMember]
		public int NSDeltaTime { get; set; }

		[DataMember]
		public List<Guid> NSDeviceUIDs { get; set; }

		[DataMember]
		public XDeviceLogic StartLogic { get; set; }

		[DataMember]
		public XDeviceLogic ForbidLogic { get; set; }

		[DataMember]
		public XDeviceLogic StopLogic { get; set; }

		public override string PresentationName
		{
			get { return No + "." + Name; }
		}

		public override string DescriptorInfo
		{
			get { return "НС " + PresentationName; }
		}

		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}
		public event Action Changed;
	}
}