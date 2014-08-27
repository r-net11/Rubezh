using System.Runtime.Serialization;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using System;
using System.Collections.Generic;

namespace SKDModule.Model
{
	public class TimeTrackFilter : OrganisationFilterBase
	{
		public TimeTrackFilter()
			: base()
		{
			EmployeeFilter = new EmployeeFilter();
			var hasEmployeePermission = FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Employees);
			var hasGuestPermission = FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Guests);
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