using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD.ReportFilters
{
	[DataContract]
	public class ReportFilter421 : SKDReportFilter, IReportFilterPeriod, IReportFilterOrganisation, IReportFilterDepartment, IReportFilterEmployee, IReportFilterScheduleScheme
	{
		public ReportFilter421()
		{
			ShowDelay = true;
			ShowEarlуRetirement = true;
			ShowTolerance = true;
			ShowAbsence = true;
			ShowOvertime = true;
			ShowMissingtime = true;
			ShowConfirmed = true;
			ShowWithoutTolerance = true;
		}

		#region IReportFilterPeriod Members

		[DataMember]
		public ReportPeriodType PeriodType { get; set; }
		[DataMember]
		public DateTime DateTimeFrom { get; set; }
		[DataMember]
		public DateTime DateTimeTo { get; set; }

		#endregion

		#region IReportFilterOrganisation Members

		[DataMember]
		public List<Guid> Organisations { get; set; }

		#endregion

		#region IReportFilterDepartment Members

		[DataMember]
		public List<Guid> Departments { get; set; }

		#endregion

		#region IReportFilterEmployee Members

		[DataMember]
		public List<Guid> Employees { get; set; }

		#endregion

		#region IReportFilterScheduleScheme Members

		[DataMember]
		public List<Guid> ScheduleSchemas { get; set; }

		#endregion

		[DataMember]
		public bool ShowAllViolation { get; set; }
		[DataMember]
		public bool ShowDelay { get; set; }
		[DataMember]
		public bool ShowEarlуRetirement { get; set; }
		[DataMember]
		public bool ShowTolerance { get; set; }
		[DataMember]
		public bool ShowAbsence { get; set; }
		[DataMember]
		public bool ShowOvertime { get; set; }
		[DataMember]
		public bool ShowMissingtime { get; set; }
		[DataMember]
		public bool ShowConfirmed { get; set; }
		[DataMember]
		public bool ShowWithoutTolerance { get; set; }

	}
}
