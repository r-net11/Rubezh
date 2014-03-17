using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace FiresecAPI
{
	[DataContract]
	public class GUD : OrganizationElementBase
	{
		public GUD()
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