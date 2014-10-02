using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKMPT : GKBase
	{
		public GKMPT()
		{
			StartLogic = new GKDeviceLogic();
			MPTDevices = new List<GKMPTDevice>();
			Delay = 10;
			Devices = new List<GKDevice>();
		}

		[XmlIgnore]
		public List<GKDevice> Devices { get; set; }

		[DataMember]
		public int Delay { get; set; }

		[DataMember]
		public int Hold { get; set; }

		[DataMember]
		public DelayRegime DelayRegime { get; set; }

		[DataMember]
		public GKDeviceLogic StartLogic { get; set; }

		[DataMember]
		public List<GKMPTDevice> MPTDevices { get; set; }

		[DataMember]
		public bool UseDoorAutomatic { get; set; }

		[DataMember]
		public bool UseDoorStop { get; set; }

		[DataMember]
		public bool UseFailureAutomatic { get; set; }

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.MPT; } }

		[XmlIgnore]
		public override string PresentationName
		{
			get { return "MПТ" + "." + No + "." + Name; }
		}
	}
}