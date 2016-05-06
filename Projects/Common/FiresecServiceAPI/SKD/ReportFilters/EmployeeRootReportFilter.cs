using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StrazhAPI.Enums;

namespace StrazhAPI.SKD.ReportFilters
{
	[DataContract]
	[KnownType(typeof(EmployeeRootReportFilter))]
	public class EmployeeRootReportFilter : SKDReportFilter, IReportFilterPeriod, IReportFilterOrganisation, IReportFilterDepartment, IReportFilterPosition, IReportFilterEmployeeAndVisitor, IReportFilterZone
	{
		public EmployeeRootReportFilter()
		{
			IsEmployee = true;
			ReportType = Enums.ReportType.EmployeeRootReport;
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

		#region IReportFilterEmployeeAndVisitor Members

		[DataMember]
		public bool IsEmployee { get; set; }

		#endregion IReportFilterEmployeeAndVisitor Members

		#region IReportFilterZone Members

		[DataMember]
		public List<Guid> Zones { get; set; }

		#endregion IReportFilterZone Members

		#region IReportFilterPeriod Members

		[DataMember]
		public ReportPeriodType PeriodType { get; set; }

		[DataMember]
		public DateTime DateTimeFrom { get; set; }

		[DataMember]
		public DateTime DateTimeTo { get; set; }

		#endregion IReportFilterPeriod Members
	}
}