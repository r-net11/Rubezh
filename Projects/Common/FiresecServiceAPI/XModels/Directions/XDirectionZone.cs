using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class XDirectionZone
	{
		public XDirectionZone()
		{
			StateBit = XStateBit.Fire1;
		}

		[XmlIgnore]
		public XZone Zone { get; set; }

		[DataMember]
		public Guid ZoneUID { get; set; }

		[DataMember]
		public XStateBit StateBit { get; set; }
	}
}