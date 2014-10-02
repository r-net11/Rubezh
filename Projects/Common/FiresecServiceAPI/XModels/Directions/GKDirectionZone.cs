using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKDirectionZone
	{
		public GKDirectionZone()
		{
			StateBit = GKStateBit.Fire1;
		}

		[XmlIgnore]
		public GKZone Zone { get; set; }

		[DataMember]
		public Guid ZoneUID { get; set; }

		[DataMember]
		public GKStateBit StateBit { get; set; }
	}
}