using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace FiresecAPI
{
	[DataContract]
	public class GUD
	{
		public GUD()
		{
			UID = Guid.NewGuid();
			CardZones = new List<CardZone>();
		}

		[DataMember]
		public Guid UID;

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<CardZone> CardZones { get; set; }
	}
}