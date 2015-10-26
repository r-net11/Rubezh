using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhClient;

namespace SKDModule.Model
{
	public class TimeTrackFilter : OrganisationFilterBase
	{
		public TimeTrackFilter()
			: base()
		{
			EmployeeFilter = new EmployeeFilter();
			var hasEmployeePermission = ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Employees_View);
			var hasGuestPermission = ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Guests_View);
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