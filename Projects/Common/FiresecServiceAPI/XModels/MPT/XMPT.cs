using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
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

		[DataMember]
		public int Delay { get; set; }

		[DataMember]
		public int Hold { get; set; }

		[DataMember]
		public DelayRegime DelayRegime { get; set; }

		[DataMember]
		public XDeviceLogic StartLogic { get; set; }

		[DataMember]
		public List<MPTDevice> MPTDevices { get; set; }

		[DataMember]
		public bool UseDoorAutomatic { get; set; }

		[DataMember]
		public bool UseDoorStop { get; set; }

		[DataMember]
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