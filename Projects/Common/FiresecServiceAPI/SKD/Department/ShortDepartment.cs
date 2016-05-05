using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class ShortDepartment : IOrganisationElement
	{
		public ShortDepartment()
		{
			ChildDepartments = new Dictionary<Guid, string>();
			ParentDepartments = new Dictionary<Guid, string>();
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
		public Dictionary<Guid, string> ChildDepartments { get; set; }

		[DataMember]
		public Dictionary<Guid, string> ParentDepartments { get; set; }

		[DataMember]
		public Guid OrganisationUID { get; set; }

		[DataMember]
		public Guid ChiefUID { get; set; }

		[DataMember]
		public string Phone { get; set; }

		[DataMember]
		public bool IsDeleted { get; set; }

		[DataMember]
		public DateTime RemovalDate { get; set; }
	}
}