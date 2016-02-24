using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Enums;

namespace FiresecAPI.SKD.ReportFilters
{
	[DataContract]
	[KnownType(typeof(WorkingTimeReportFilter))]
	public class WorkingTimeReportFilter : SKDReportFilter, IReportFilterPeriod, IReportFilterOrganisation, IReportFilterDepartment, IReportFilterPosition, IReportFilterEmployee
	{
		public WorkingTimeReportFilter()
		{
			ReportType = ReportType.WorkingTimeReport;
		}

		#region IReportFilterOrganisation Members

		[DataMember]
		public List<Guid> Organisations { get; set; }

		#endregion IReportFilterOrganisation Members

		#region IReportFilterDepartment Members

		[DataMember]
		public List<Guid> Departments { get; set; }

		#endregion IReportFilterDepartment Members

		#region IReportFilterPosition Members

		[DataMember]
		public List<Guid> Positions { get; set; }

		#endregion IReportFilterPosition Members

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

		#region IReportFilterPeriod Members

		[DataMember]
		public ReportPeriodType PeriodType { get; set; }

		[DataMember]
		public DateTime DateTimeFrom { get; set; }

		[DataMember]
		public DateTime DateTimeTo { get; set; }

		#endregion IReportFilterPeriod Members

		/// <summary>
		/// Учибывать только сверхурочные. Для варианта расчета баланса
		/// </summary>
		[DataMember]
		public bool AllowOnlyAcceptedOvertime { get; set; }
	}
}