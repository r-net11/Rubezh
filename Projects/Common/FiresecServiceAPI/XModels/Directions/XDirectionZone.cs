using System;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class XDirectionZone
	{
		public XDirectionZone()
		{
			StateBit = XStateBit.Fire1;
		}

		public XZone Zone { get; set; }

		[DataMember]
		public Guid ZoneUID { get; set; }

		[DataMember]
		public XStateBit StateBit { get; set; }
	}
}