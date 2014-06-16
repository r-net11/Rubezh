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
			DepartmentFilter = new DepartmentFilter();
			PositionFilter = new PositionFilter();
			CardFilter = new CardFilter();
		}

		[DataMember]
		public EmployeeFilter EmployeeFilter { get; set; }

		[DataMember]
		public DepartmentFilter DepartmentFilter { get; set; }

		[DataMember]
		public PositionFilter PositionFilter { get; set; }

		[DataMember]
		public CardFilter CardFilter { get; set; }
	}
}