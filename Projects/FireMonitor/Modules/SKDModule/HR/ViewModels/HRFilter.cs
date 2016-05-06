using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class HRFilter : OrganisationFilterBase
	{
		public HRFilter()
			: base()
		{
			EmployeeFilter = new EmployeeFilter();
			var hasEmployeePermission = FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Employees_View);
			var hasGuestPermission = FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Guests_View);
			EmployeeFilter.PersonType = hasGuestPermission && !hasEmployeePermission ? PersonType.Guest : PersonType.Employee;
		}

		[DataMember]
		public EmployeeFilter EmployeeFilter { get; set; }

		public List<Guid> EmplooyeeUIDs { get { return EmployeeFilter.UIDs; } }
	}
}