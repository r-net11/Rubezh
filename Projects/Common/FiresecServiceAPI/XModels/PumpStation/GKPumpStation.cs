using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKPumpStation : GKBase
	{
		public GKPumpStation()
		{
			Delay = 10;
			Hold = 60;
			DelayRegime = DelayRegime.Off;
			NSPumpsCount = 1;
			NSDeltaTime = 15;
			StartLogic = new GKDeviceLogic();
			StopLogic = new GKDeviceLogic();
			AutomaticOffLogic = new GKDeviceLogic();

			NSDevices = new List<GKDevice>();
			NSDeviceUIDs = new List<Guid>();
		}

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.PumpStation; } }
		[XmlIgnore]
		public List<GKDevice> NSDevices { get; set; }

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
		public GKDeviceLogic StartLogic { get; set; }

		[DataMember]
		public GKDeviceLogic StopLogic { get; set; }

		[DataMember]
		public GKDeviceLogic AutomaticOffLogic { get; set; }

		[XmlIgnore]
		public override string PresentationName
		{
			get { return "0" + No + "." + Name; }
		}
	}
}