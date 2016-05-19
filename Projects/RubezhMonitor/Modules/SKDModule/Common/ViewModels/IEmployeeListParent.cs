using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKDModule.ViewModels
{
	public interface IEmployeeListParent
	{
		bool IsDeleted { get; set; }
		bool IsOrganisationDeleted { get; set; }
		bool IsWithDeleted { get; }
		Guid UID { get; }
		Guid OrganisationUID { get; }
		string Name { get; }
		string Description { get; }
	}
}
