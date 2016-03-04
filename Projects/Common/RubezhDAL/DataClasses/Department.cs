using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubezhDAL.DataClasses
{
	public class Department : IOrganisationItem, IExternalKey
	{
		public Department()
		{
			ChildDepartments = new List<Department>();
			Employees = new List<Employee>();
		}

		#region IOrganisationItemMembers
		[Key]
		public Guid UID { get; set; }
		[MaxLength(50)]
		public string Name { get; set; }
		[MaxLength(4000)]
		public string Description { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime? RemovalDate { get; set; }
		[Index]
		public Guid? OrganisationUID { get; set; }
		public Organisation Organisation { get; set; }
		#endregion

		public Guid? PhotoUID { get; set; }
		public Photo Photo { get; set; }
		[Index]
		public Guid? ParentDepartmentUID { get; set; }
		[ForeignKey("ParentDepartmentUID")]
		public Department ParentDepartment { get; set; }
		[InverseProperty("ParentDepartment")]
		public ICollection<Department> ChildDepartments { get; set; }

		[InverseProperty("Department")]
		public ICollection<Employee> Employees { get; set; }

		public Guid? ChiefUID { get; set; }
		public Employee Chief { get; set; }
		[MaxLength(50)]
		public string ExternalKey { get; set; }

		public string Phone { get; set; }
	}
}
