using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
    public class ShortDepartment : IOrganisationElement
	{
		public ShortDepartment()
		{
            ChildDepartmentUIDs = new List<Guid>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public Guid? ParentDepartmentUID { get; set; }

		[DataMember]
		public List<Guid> ChildDepartmentUIDs { get; set; }

		[DataMember]
		public Guid OrganisationUID { get; set; }
	}
}