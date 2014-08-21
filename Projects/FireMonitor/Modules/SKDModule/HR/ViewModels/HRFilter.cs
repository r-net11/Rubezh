using System.Runtime.Serialization;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class HRFilter : OrganisationFilterBase
	{
		public HRFilter()
			: base()
		{
			EmployeeFilter = new EmployeeFilter();
			var hasEmployeePermission = FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Employees);
			var hasGuestPermission = FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Guests);
			EmployeeFilter.PersonType = hasGuestPermission && !hasEmployeePermission ? PersonType.Guest : PersonType.Employee;
		}

		[DataMember]
		public EmployeeFilter EmployeeFilter { get; set; }
	}
}