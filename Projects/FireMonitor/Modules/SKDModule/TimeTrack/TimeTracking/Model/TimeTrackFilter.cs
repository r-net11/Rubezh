using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using FiresecClient;

namespace SKDModule.Model
{
	public class TimeTrackFilter : OrganisationFilterBase
	{
		public TimeTrackFilter()
		{
			EmployeeFilter = new EmployeeFilter();
			var hasEmployeePermission = FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Employees_View);
			var hasGuestPermission = FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Guests_View);
			EmployeeFilter.PersonType = hasGuestPermission && !hasEmployeePermission ? PersonType.Guest : PersonType.Employee;
			Period = TimeTrackingPeriod.CurrentMonth;
			TotalTimeTrackTypeFilters = new List<TimeTrackType>();
		}

		[DataMember]
		public EmployeeFilter EmployeeFilter { get; set; }

		public TimeTrackingPeriod Period { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public List<TimeTrackType> TotalTimeTrackTypeFilters { get; set; }
	}
}