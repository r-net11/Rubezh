using System.Runtime.Serialization;
using System.Collections.Generic;
using System;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class Password : OrganisationElementBase
	{
		public Password()
		{
			//GuardZoneUIDs = new List<Guid>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string PasswordString { get; set; }
	}
}