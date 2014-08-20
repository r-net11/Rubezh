using System.Runtime.Serialization;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using System;

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
		}

		[DataMember]
		public EmployeeFilter EmployeeFilter { get; set; }


		public TimeTrackingPeriod Period { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public bool IsTotal { get; set; }
		public bool IsTotalMissed { get; set; }
		public bool IsTotalInSchedule { get; set; }
		public bool IsTotalOvertime { get; set; }
		public bool IsTotalLate { get; set; }
		public bool IsTotalEarlyLeave { get; set; }
		public bool IsTotalPlanned { get; set; }
		public bool IsTotalEavening { get; set; }
		public bool IsTotalNight { get; set; }
		public bool IsTotal_DocumentOvertime { get; set; }
		public bool IsTotal_DocumentPresence { get; set; }
		public bool IsTotal_DocumentAbsence { get; set; }
	}
}