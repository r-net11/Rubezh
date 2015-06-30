using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class AdditionalColumnType : IOrganisationItem
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

		public ICollection<AdditionalColumn> AdditionalColumns { get; set; }

		public int DataType { get; set; }

		public int PersonType { get; set; }

		public bool IsInGrid { get; set; }
	}
}
