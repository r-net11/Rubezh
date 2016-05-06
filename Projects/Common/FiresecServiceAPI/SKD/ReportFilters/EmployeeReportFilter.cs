using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StrazhAPI.Enums;

namespace StrazhAPI.SKD.ReportFilters
{
	[DataContract]
	[KnownType(typeof(EmployeeReportFilter))]
	public class EmployeeReportFilter : SKDReportFilter, IReportFilterOrganisation, IReportFilterDepartment, IReportFilterPosition, IReportFilterEmployeeAndVisitor, IReportFilterArchive
	{
		public EmployeeReportFilter()
		{
			IsEmployee = true;
			ReportType = Enums.ReportType.EmployeeReport;
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

		#region IReportFilterArchive Members

		[DataMember]
		public bool UseArchive { get; set; }

		#endregion IReportFilterArchive Members
	}
}