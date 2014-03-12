using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XMPT : XBase
	{
		public XMPT()
		{
			StartLogic = new XDeviceLogic();
			MPTDevices = new List<MPTDevice>();

			InputDevices = new List<XDevice>();
			InputZones = new List<XZone>();
			InputDirections = new List<XDirection>();
			Devices = new List<XDevice>();
		}

		public List<XDevice> InputDevices { get; set; }
		public List<XZone> InputZones { get; set; }
		public List<XDirection> InputDirections { get; set; }
		public List<XDevice> Devices { get; set; }

		[DataMember]
		public string Name { get; set; }

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

		public override XBaseObjectType ObjectType { get { return XBaseObjectType.MPT; } }

		public override string PresentationName
		{
			get { return "MПТ" + "." + Name; }
		}
	}
}