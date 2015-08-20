﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class ShortDepartment : IOrganisationElement
	{
		public ShortDepartment()
		{
			ChildDepartments = new List<TinyDepartment>();
			ParentDepartments = new List<TinyDepartment>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public Guid ParentDepartmentUID { get; set; }

		[DataMember]
		public List<TinyDepartment> ChildDepartments { get; set; }

		[DataMember]
		public List<TinyDepartment> ParentDepartments { get; set; }

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

	[DataContract]
	public class TinyDepartment
	{
		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }
	}
}