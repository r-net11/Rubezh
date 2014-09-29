using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	public class XMPT : XBase
	{
		public XMPT()
		{
			StartLogic = new XDeviceLogic();
			MPTDevices = new List<MPTDevice>();
			Delay = 10;
			Devices = new List<XDevice>();
		}

		[XmlIgnore]
		public List<XDevice> Devices { get; set; }

		public int Delay { get; set; }
		public int Hold { get; set; }
		public DelayRegime DelayRegime { get; set; }
		public XDeviceLogic StartLogic { get; set; }
		public List<MPTDevice> MPTDevices { get; set; }
		public bool UseDoorAutomatic { get; set; }
		public bool UseDoorStop { get; set; }
		public bool UseFailureAutomatic { get; set; }

		[XmlIgnore]
		public override XBaseObjectType ObjectType { get { return XBaseObjectType.MPT; } }

		[XmlIgnore]
		public override string PresentationName
		{
			get { return "MПТ" + "." + No + "." + Name; }
		}
	}
}