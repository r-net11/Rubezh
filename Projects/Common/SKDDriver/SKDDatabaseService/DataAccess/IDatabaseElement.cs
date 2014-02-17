using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKDDriver.DataAccess
{
	public interface IDatabaseElement
	{
		Guid Uid { get; set; }
		bool? IsDeleted { get; set; }
		DateTime? RemovalDate { get; set; }
	}

	public interface IOrganizationDatabaseElement:IDatabaseElement
	{
		Guid? OrganizationUid { get; set; }
	}
}
