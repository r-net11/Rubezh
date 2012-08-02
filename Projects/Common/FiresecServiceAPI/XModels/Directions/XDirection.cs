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
			ZoneNos = new List<int>();
		}

		public List<XZone> Zones { get; set; }

		[DataMember]
		public short No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<int> ZoneNos { get; set; }

		public string PresentationName
		{
			get { return Name + " - " + No.ToString(); }
		}
	}
}