using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class Organisation : SKDIsDeletedModel
	{
		public Organisation()
		{
			ZoneUIDs = new List<Guid>();
			UserUIDs = new List<Guid>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public Guid? PhotoUID { get; set; }

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> UserUIDs { get; set; }
	}

	[DataContract]
	public class OrganisationDetails : SKDIsDeletedModel
	{
		public OrganisationDetails()
		{
			ZoneUIDs = new List<Guid>();
			UserUIDs = new List<Guid>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public Photo Photo { get; set; }

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> UserUIDs { get; set; }
	}

}