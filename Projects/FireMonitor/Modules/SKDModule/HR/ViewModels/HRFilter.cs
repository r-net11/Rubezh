using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhClient;

namespace SKDModule.ViewModels
{
	public class HRFilter : OrganisationFilterBase
	{
		public HRFilter()
			: base()
		{
			EmployeeFilter = new EmployeeFilter();
			var hasEmployeePermission = ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Employees_View);
			var hasGuestPermission = ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Guests_View);
			EmployeeFilter.PersonType = hasGuestPermission && !hasEmployeePermission ? PersonType.Guest : PersonType.Employee;
		}

		[DataMember]
		public EmployeeFilter EmployeeFilter { get; set; }

		public List<Guid> EmplooyeeUIDs { get { return EmployeeFilter.UIDs; } }
	}
}