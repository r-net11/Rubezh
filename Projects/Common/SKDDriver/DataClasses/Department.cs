using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDDriver.DataClasses
{
	public class Department : IOrganisationItem
	{
		public Department()
		{
			ChildDepartments = new List<Department>();
			Employees = new List<Employee>();
		}
		
		#region IOrganisationItemMembers
		[Key]
		public Guid UID { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime? RemovalDate { get; set; }

		public Guid? OrganisationUID { get; set; }
		public Organisation Organisation { get; set; }
		#endregion

		public Guid? PhotoUID { get; set; }
		public Photo Photo { get; set; }

		public Guid? ParentDepartmentUID { get; set; }
		[ForeignKey("ParentDepartmentUID")]
		public Department ParentDepartment { get; set; }
		[InverseProperty("ParentDepartment")]
		public ICollection<Department> ChildDepartments { get; set; }

        [InverseProperty("Department")]
		public ICollection<Employee> Employees { get; set; }

		public Guid? ChiefUID { get; set; }
		public Employee Chief { get; set; }

		public string ExternalKey { get; set; }

		public string Phone { get; set; }
	}
}
