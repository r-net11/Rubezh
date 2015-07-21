using System;

namespace SKDDriver.DataClasses
{
	public interface IOrganisationItem
	{
		Guid UID { get; set; }

		string Name { get; set; }

		string Description { get; set; }

		bool IsDeleted { get; set; }

		DateTime? RemovalDate { get; set; }

		Guid? OrganisationUID { get; set; }

		Organisation Organisation { get; set; }
	}
}
