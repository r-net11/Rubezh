using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD.ReportFilters
{
	[DataContract]
	public class DisciplineReportFilter : SKDReportFilter, IReportFilterPeriod, IReportFilterOrganisation, IReportFilterDepartment, IReportFilterEmployee, IReportFilterScheduleScheme
	{
		public DisciplineReportFilter()
		{
			ShowDelay = true;
			ShowEarlуRetirement = true;
			ShowAbsence = true;
			ShowOvertime = true;
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
		[DataMember]
		public bool IsSearch { get; set; }
		[DataMember]
		public string LastName { get; set; }
		[DataMember]
		public string FirstName { get; set; }
		[DataMember]
		public string SecondName { get; set; }

		#endregion

		#region IReportFilterScheduleScheme Members

		[DataMember]
		public List<Guid> ScheduleSchemas { get; set; }

		#endregion

		[DataMember]
		public bool ShowDelay { get; set; }
		[DataMember]
		public bool ShowEarlуRetirement { get; set; }
		[DataMember]
		public bool ShowAbsence { get; set; }
		[DataMember]
		public bool ShowOvertime { get; set; }
		[DataMember]
		public bool ShowConfirmed { get; set; }
		[DataMember]
		public bool ShowWithoutTolerance { get; set; }

	}
}