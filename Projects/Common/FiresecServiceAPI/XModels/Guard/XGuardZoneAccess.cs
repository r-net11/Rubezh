using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class XGuardZoneAccess
	{
		public XGuardZoneAccess()
		{
		}

		[DataMember]
		public Guid ZoneUID { get; set; }

		[DataMember]
		public XGuardZoneAccessType GuardZoneAccessType { get; set; }
	}
}