using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class Holiday : IOrganisationItem
	{
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

		public int Type { get; set; }

		public DateTime Date { get; set; }

		public DateTime? TransferDate { get; set; }

		public int Reduction { get; set; }
	}
}
