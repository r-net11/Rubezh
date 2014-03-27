using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class AccessTemplate : OrganizationElementBase
	{
		public AccessTemplate()
			: base()
		{
			CardZones = new List<CardZone>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<CardZone> CardZones { get; set; }
	}
}