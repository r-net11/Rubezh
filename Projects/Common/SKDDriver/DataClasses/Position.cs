using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDDriver.DataClasses
{
    public class Position : IOrganisationItem, IExternalKey
	{
		public Position()
		{
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

        [InverseProperty("Position")]
		public ICollection<Employee> Employees { get; set; }

		public string ExternalKey { get; set; }
	}
}
