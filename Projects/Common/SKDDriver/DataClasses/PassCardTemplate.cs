using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class PassCardTemplate : IOrganisationItem
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

		public byte[] Data { get; set; }
	}
}
