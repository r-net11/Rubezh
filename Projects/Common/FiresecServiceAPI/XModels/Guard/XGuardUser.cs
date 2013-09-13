using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XGuardUser
	{
		public XGuardUser()
		{
			UID = Guid.NewGuid();
			ZoneUIDs = new List<Guid>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public string FIO { get; set; }

		[DataMember]
		public string Function { get; set; }

		[DataMember]
		public bool CanSetZone { get; set; }

		[DataMember]
		public bool CanUnSetZone { get; set; }

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		public List<XZone> Zones { get; set; }
	}
}