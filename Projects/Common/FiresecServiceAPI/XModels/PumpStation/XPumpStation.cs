using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	public class XPumpStation : XBase
	{
		public XPumpStation()
		{
			Delay = 10;
			Hold = 60;
			DelayRegime = DelayRegime.Off;
			NSPumpsCount = 1;
			NSDeltaTime = 15;
			StartLogic = new XDeviceLogic();
			StopLogic = new XDeviceLogic();
			AutomaticOffLogic = new XDeviceLogic();

			NSDevices = new List<XDevice>();
			NSDeviceUIDs = new List<Guid>();
		}

		[XmlIgnore]
		public override XBaseObjectType ObjectType { get { return XBaseObjectType.PumpStation; } }
		[XmlIgnore]
		public List<XDevice> NSDevices { get; set; }

		public ushort Delay { get; set; }
		public ushort Hold { get; set; }
		public DelayRegime DelayRegime { get; set; }
		public int NSPumpsCount { get; set; }
		public int NSDeltaTime { get; set; }
		public List<Guid> NSDeviceUIDs { get; set; }
		public XDeviceLogic StartLogic { get; set; }
		public XDeviceLogic StopLogic { get; set; }
		public XDeviceLogic AutomaticOffLogic { get; set; }

		[XmlIgnore]
		public override string PresentationName
		{
			get { return "0" + No + "." + Name; }
		}
	}
}