using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class Organization : SKDIsDeletedModel
	{
		public Organization()
		{
			ZoneUIDs = new List<Guid>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public Guid? PhotoUID { get; set; }

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }
	}
}