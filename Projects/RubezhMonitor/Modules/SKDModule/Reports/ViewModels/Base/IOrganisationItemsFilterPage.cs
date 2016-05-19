using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKDModule
{
	public interface IOrganisationItemsFilterPage
	{
		List<Guid> OrganisationUIDs { get; set; }
		bool IsWithDeleted { get; set; }
		void InitializeFilter();
	}
}
