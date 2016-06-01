using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StrazhAPI.Enums;

namespace StrazhAPI.SKD.ReportFilters
{
	[DataContract]
	[KnownType(typeof(DisciplineReportFilter))]
	public class DisciplineReportFilter : SKDReportFilter, IReportFilterPeriod, IReportFilterOrganisation, IReportFilterDepartment, IReportFilterEmployee, IReportFilterScheduleScheme
	{
		public DisciplineReportFilter()
		{
			ShowLate = true;
			ShowEarlуLeave = true;
			ShowAbsence = true;
			ShowOvertime = true;
			ShowAllViolation = true;
			ReportType = ReportType.DisciplineReport;
		}

		#region IReportFilterPeriod Members

		[DataMember]
		public ReportPeriodType PeriodType { get; set; }

		[DataMember]
		public DateTime DateTimeFrom { get; set; }

		[DataMember]
		public DateTime DateTimeTo { get; set; }

		#endregion IReportFilterPeriod Members

		#region IReportFilterOrganisation Members

		[DataMember]
		public List<Guid> Organisations { get; set; }

		#endregion IReportFilterOrganisation Members

		#region IReportFilterDepartment Members

		[DataMember]
		public List<Guid> Departments { get; set; }

		#endregion IReportFilterDepartment Members

		#region IReportFilterEmployee Members

		[DataMember]
		public List<Guid> Employees { get; set; }

		[DataMember]
		public bool IsSearch { get; set; }

		[DataMember]
		public string LastName { get; set; }

		[DataMember]
		public string FirstName { get; set; }

		[DataMember]
		public string SecondName { get; set; }

		#endregion IReportFilterEmployee Members

		#region IReportFilterScheduleScheme Members

		[DataMember]
		public List<Guid> ScheduleSchemas { get; set; }

		#endregion IReportFilterScheduleScheme Members

		[DataMember]
		public bool ShowAllViolation { get; set; }

		[DataMember]
		public bool ShowShiftedViolation { get; set; }

		[DataMember]
		public bool ShowLate { get; set; }

		[DataMember]
		public bool ShowEarlуLeave { get; set; }

		[DataMember]
		public bool ShowAbsence { get; set; }

		[DataMember]
		public bool ShowOvertime { get; set; }
	}
}