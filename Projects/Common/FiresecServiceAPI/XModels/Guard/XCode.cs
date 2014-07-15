using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class XCode
	{
		public XCode()
		{
			UID = Guid.NewGuid();
			GuardZoneUIDs = new List<Guid>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public bool CanSetZone { get; set; }

		[DataMember]
		public bool CanUnSetZone { get; set; }

		[DataMember]
		public List<Guid> GuardZoneUIDs { get; set; }

		public List<XGuardZone> GuardZones { get; set; }
	}
}