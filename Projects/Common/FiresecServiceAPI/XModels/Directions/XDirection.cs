using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XDirection : XBinaryBase
	{
		public XDirection()
		{
			Zones = new List<int>();
			DirectionDevices = new List<DirectionDevice>();
		}

		[DataMember]
		public short No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<int> Zones { get; set; }

		[DataMember]
		public List<DirectionDevice> DirectionDevices { get; set; }

		public string PresentationName
		{
			get { return Name + " - " + No.ToString(); }
		}
	}
}