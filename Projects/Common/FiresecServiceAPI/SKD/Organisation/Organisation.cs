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
			DoorUIDs = new List<Guid>();
			GKDoorUIDs = new List<Guid>();
			UserUIDs = new List<Guid>();
			GuardZoneUIDs = new List<Guid>();
			ZoneUIDs = new List<Guid>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public Guid? PhotoUID { get; set; }

		[DataMember]
		public List<Guid> DoorUIDs { get; set; }

		[DataMember]
		public List<Guid> GKDoorUIDs { get; set; }

		[DataMember]
		public List<Guid> GuardZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> UserUIDs { get; set; }

		[DataMember]
		public Guid ChiefUID { get; set; }

		[DataMember]
		public string Phone { get; set; }

		[DataMember]
		public Guid HRChiefUID { get; set; }
	}

	[DataContract]
	public class OrganisationDetails : SKDIsDeletedModel
	{
		public OrganisationDetails()
		{
			DoorUIDs = new List<Guid>();
			UserUIDs = new List<Guid>();
			GuardZoneUIDs = new List<Guid>();
			ZoneUIDs = new List<Guid>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public Photo Photo { get; set; }

		[DataMember]
		public List<Guid> DoorUIDs { get; set; }

		[DataMember]
		public List<Guid> GuardZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> UserUIDs { get; set; }

		[DataMember]
		public Guid ChiefUID { get; set; }

		[DataMember]
		public Guid HRChiefUID { get; set; }

		[DataMember]
		public string Phone { get; set; }
	}
}