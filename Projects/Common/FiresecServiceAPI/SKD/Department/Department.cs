using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class Department : OrganisationElementBase
	{
		public Department()
			: base()
		{
			ChildDepartmentUIDs = new List<Guid>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public Guid? ParentDepartmentUID { get; set; } //данное поле nullable для совместимости с предыдущими версиями. В новых версиях значение поля никогда не будет являться null.

		[DataMember]
		public List<Guid> ChildDepartmentUIDs { get; set; }

		[DataMember]
		public Guid? ContactEmployeeUID { get; set; }

		[DataMember]
		public Guid? AttendantEmployeeUID { get; set; }

		[DataMember]
		public Photo Photo { get; set; }

		[DataMember]
		public string Phone { get; set; }

		[DataMember]
		public Guid ChiefUID { get; set; }
	}
}