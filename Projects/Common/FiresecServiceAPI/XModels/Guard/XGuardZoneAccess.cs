using System;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class XGuardZoneAccess
	{
		[DataMember]
		public Guid ZoneUID { get; set; }

		[DataMember]
		public bool CanSet { get; set; }

		[DataMember]
		public bool CanReset { get; set; }
	}
}