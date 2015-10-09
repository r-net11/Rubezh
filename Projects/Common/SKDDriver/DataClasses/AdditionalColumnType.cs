using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubezhDAL.DataClasses
{
	public class AdditionalColumnType : IOrganisationItem
	{
		public AdditionalColumnType()
		{
			AdditionalColumns = new List<AdditionalColumn>();
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

		public ICollection<AdditionalColumn> AdditionalColumns { get; set; }

		public int DataType { get; set; }

		public int PersonType { get; set; }

		public bool IsInGrid { get; set; }
	}
}
